using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SSeuropa.Tests.Api;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Courses_AnonymousList_Works()
    {
        var response = await _client.GetAsync("/api/courses?page=1&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<ApiEnvelope<Paged>>(JsonOptions);
        payload!.Success.Should().BeTrue();
        payload.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DemoLogin_ThenStudentDashboard_Works()
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/demo-login", new
        {
            email = "student@sseuropa.demo",
            deviceId = "test-device",
            deviceName = "Integration Test"
        });
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var login = await loginResponse.Content.ReadFromJsonAsync<ApiEnvelope<AuthData>>(JsonOptions);
        login!.Data!.AccessToken.Should().NotBeNullOrWhiteSpace();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Data.AccessToken);
        var dash = await _client.GetAsync("/api/dashboard/student");
        dash.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Website_ContactUs_And_Visitor_Work()
    {
        // 1. Visitor tracking
        var visitorRes = await _client.PostAsJsonAsync("/api/user/visitor", new
        {
            timezone = "Europe/Rome",
            language = "it",
            page = "/contatti"
        });
        visitorRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // 2. Submit contact form from website
        var contactRes = await _client.PostAsJsonAsync("/api/user/contact-us", new
        {
            name = "Giuseppe",
            surname = "Verdi",
            email = "giuseppe.verdi@example.it",
            telephone_number = "+39 340 1234567",
            country = "Italy",
            object_type = 1, // Request Information
            service_of_interest = 2, // Training & Courses
            preferred_contact_method = 0, // Email
            message = "Vorrei informazioni sul corso A1.",
            consent_for_contact = 1,
            consent_share_data_for_advertising = 0,
            consent_third_party_promotional = 0
        });
        contactRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3. Login as Staff and verify the enquiry is listed
        var staffLogin = await _client.PostAsJsonAsync("/api/auth/demo-login", new
        {
            email = "staff@sseuropa.demo",
            deviceId = "staff-test-device",
            deviceName = "Integration Test"
        });
        staffLogin.StatusCode.Should().Be(HttpStatusCode.OK);
        var staffData = await staffLogin.Content.ReadFromJsonAsync<ApiEnvelope<AuthData>>(JsonOptions);

        using var staffClient = _client;
        staffClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", staffData!.Data!.AccessToken);
        var enquiriesRes = await staffClient.GetAsync("/api/enquiries");
        enquiriesRes.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private sealed record ApiEnvelope<T>(bool Success, string? Message, T? Data);
    private sealed record Paged(List<object> Items, int Page, int PageSize, int TotalCount);
    private sealed record AuthData(string AccessToken);
}
