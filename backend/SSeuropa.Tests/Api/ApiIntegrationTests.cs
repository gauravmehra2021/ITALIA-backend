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

    private sealed record ApiEnvelope<T>(bool Success, string? Message, T? Data);
    private sealed record Paged(List<object> Items, int Page, int PageSize, int TotalCount);
    private sealed record AuthData(string AccessToken);
}
