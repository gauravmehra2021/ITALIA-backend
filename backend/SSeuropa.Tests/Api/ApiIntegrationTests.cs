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
        _client = factory.WithWebHostBuilder(builder =>
            builder.UseSetting("Database:Provider", "InMemory")).CreateClient();
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

        // 2. Submit contact form from website (multi-service + preferred language)
        var contactRes = await _client.PostAsJsonAsync("/api/user/contact-us", new
        {
            name = "Giuseppe",
            surname = "Verdi",
            email = "giuseppe.verdi@example.it",
            telephone_number = "",
            country = "Italy",
            object_type = 1, // Request Information
            service_of_interest = 2, // Training & Courses
            services_of_interest = new[] { 2, 1 },
            preferred_contact_method = 0, // Email
            preferred_language = "English",
            message = "Vorrei informazioni sul corso A1.",
            consent_for_contact = 1,
            consent_share_data_for_advertising = 0,
            consent_third_party_promotional = 0
        });
        contactRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3. Login as Staff and verify the enquiry is listed with expected fields
        var staffLogin = await _client.PostAsJsonAsync("/api/auth/demo-login", new
        {
            email = "staff@sseuropa.demo",
            deviceId = "staff-test-device",
            deviceName = "Integration Test"
        });
        staffLogin.StatusCode.Should().Be(HttpStatusCode.OK);
        var staffData = await staffLogin.Content.ReadFromJsonAsync<ApiEnvelope<AuthData>>(JsonOptions);

        using var staffRequest = new HttpRequestMessage(HttpMethod.Get, "/api/enquiries");
        staffRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", staffData!.Data!.AccessToken);
        var enquiriesRes = await _client.SendAsync(staffRequest);
        enquiriesRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var enquiriesPayload = await enquiriesRes.Content.ReadFromJsonAsync<ApiEnvelope<List<EnquiryRecord>>>(JsonOptions);
        enquiriesPayload!.Data.Should().Contain(e =>
            e.Email == "giuseppe.verdi@example.it" &&
            e.FullName == "Giuseppe Verdi" &&
            e.InterestedCourse == "Training & Courses, CAF & Patronato" &&
            e.PreferredLanguage == "English");

        // 4. Verify Staff Dashboard endpoint
        using var staffDashboardReq = new HttpRequestMessage(HttpMethod.Get, "/api/dashboard/staff");
        staffDashboardReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", staffData.Data.AccessToken);
        var staffDashRes = await _client.SendAsync(staffDashboardReq);
        staffDashRes.StatusCode.Should().Be(HttpStatusCode.OK);

        var staffDashPayload = await staffDashRes.Content.ReadFromJsonAsync<ApiEnvelope<StaffDashboardRecord>>(JsonOptions);
        staffDashPayload!.Data.Should().NotBeNull();
        staffDashPayload.Data!.TotalEnquiries.Should().BeGreaterThan(0);
        staffDashPayload.Data.NewEnquiries.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Website_ContactUs_WithoutConsentOrEmail_ReturnsBadRequest()
    {
        var invalidRes = await _client.PostAsJsonAsync("/api/user/contact-us", new
        {
            name = "Mario",
            surname = "Rossi",
            email = "not-an-email",
            telephone_number = "",
            country = "Italy",
            object_type = 0,
            service_of_interest = 0,
            preferred_contact_method = 0,
            message = "Need info",
            consent_for_contact = 0,
            consent_share_data_for_advertising = 0,
            consent_third_party_promotional = 0
        });

        invalidRes.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MobileV1_FullStudentFlow_Works()
    {
        // 1. Mobile login challenge
        var loginRes = await _client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "student@sseuropa.demo",
            password = "Demo@12345"
        });
        loginRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var challenge = await loginRes.Content.ReadFromJsonAsync<MobileChallengeRecord>(JsonOptions);
        challenge.Should().NotBeNull();
        challenge!.ChallengeId.Should().NotBeNullOrWhiteSpace();

        // 2. Mobile verify OTP (supports demo code 123456)
        var verifyRes = await _client.PostAsJsonAsync("/api/v1/auth/otp/verify", new
        {
            challengeId = challenge.ChallengeId,
            code = "123456"
        });
        verifyRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var session = await verifyRes.Content.ReadFromJsonAsync<MobileSessionRecord>(JsonOptions);
        session.Should().NotBeNull();
        session!.AccessToken.Should().NotBeNullOrWhiteSpace();
        session.Student.Should().NotBeNull();
        session.Student.Email.Should().Be("student@sseuropa.demo");

        using var studentClient = new HttpRequestMessage();
        studentClient.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);

        // 3. Profile
        var profileReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/students/me");
        profileReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        var profileRes = await _client.SendAsync(profileReq);
        profileRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // 4. Mobile Dashboard
        var dashReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/students/me/dashboard");
        dashReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        var dashRes = await _client.SendAsync(dashReq);
        dashRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var dashData = await dashRes.Content.ReadFromJsonAsync<MobileDashboardRecord>(JsonOptions);
        dashData.Should().NotBeNull();
        dashData!.Student.DisplayName.Should().NotBeNullOrWhiteSpace();
        dashData.WeeklyGoalMinutes.Should().BeGreaterThan(0);

        // 5. Catalogue and Enrolled Courses
        var catRes = await _client.GetAsync("/api/v1/courses");
        catRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var catalogue = await catRes.Content.ReadFromJsonAsync<List<MobileCourseRecord>>(JsonOptions);
        catalogue.Should().NotBeEmpty();

        var coursesReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/students/me/courses");
        coursesReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        var coursesRes = await _client.SendAsync(coursesReq);
        coursesRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // 6. Quizzes
        var quizzesReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/students/me/quizzes");
        quizzesReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        var quizzesRes = await _client.SendAsync(quizzesReq);
        quizzesRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // 7. Live Classes
        var classesReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/students/me/live-classes");
        classesReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        var classesRes = await _client.SendAsync(classesReq);
        classesRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // 8. Progress
        var progReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/students/me/progress");
        progReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        var progRes = await _client.SendAsync(progReq);
        progRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // 9. AI Tutor message
        var tutorReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/students/me/ai-tutor/messages")
        {
            Content = JsonContent.Create(new { message = "Come si dice 'Good morning' in italiano?" })
        };
        tutorReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        var tutorRes = await _client.SendAsync(tutorReq);
        tutorRes.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MobileV1_SignupAndForgotPasswordFlow_Works()
    {
        var testEmail = $"student_{Guid.NewGuid():N}@sseuropa.demo";

        // 1. Mobile Register (Sign Up)
        var regRes = await _client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            firstName = "Marco",
            lastName = "Polo",
            email = testEmail,
            password = "InitialPassword@123",
            preferredLanguage = "it"
        });
        regRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var regChallenge = await regRes.Content.ReadFromJsonAsync<MobileChallengeRecord>(JsonOptions);
        regChallenge.Should().NotBeNull();
        regChallenge!.ChallengeId.Should().NotBeNullOrWhiteSpace();

        // 2. Verify OTP for registered user
        var verifyRes = await _client.PostAsJsonAsync("/api/v1/auth/otp/verify", new
        {
            challengeId = regChallenge.ChallengeId,
            code = "123456"
        });
        verifyRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var session = await verifyRes.Content.ReadFromJsonAsync<MobileSessionRecord>(JsonOptions);
        session.Should().NotBeNull();
        session!.Student.DisplayName.Should().Be("Marco Polo");

        // 3. Forgot password request
        var forgotRes = await _client.PostAsJsonAsync("/api/v1/auth/forgot-password", new
        {
            email = testEmail
        });
        forgotRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var forgotChallenge = await forgotRes.Content.ReadFromJsonAsync<MobileChallengeRecord>(JsonOptions);
        forgotChallenge.Should().NotBeNull();
        forgotChallenge!.ChallengeId.Should().NotBeNullOrWhiteSpace();

        // 4. Reset password
        var resetRes = await _client.PostAsJsonAsync("/api/v1/auth/reset-password", new
        {
            challengeId = forgotChallenge.ChallengeId,
            code = "123456",
            newPassword = "UpdatedPassword@123"
        });
        resetRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // 5. Login with new password
        var loginRes = await _client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = testEmail,
            password = "UpdatedPassword@123"
        });
        loginRes.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Admin_ForgotPasswordFlow_Works()
    {
        var email = "admin@sseuropa.demo";

        // 1. Forgot password request
        var forgotRes = await _client.PostAsJsonAsync("/api/auth/forgot-password", new
        {
            email
        });
        forgotRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var forgotEnvelope = await forgotRes.Content.ReadFromJsonAsync<ApiEnvelope<MobileChallengeRecord>>(JsonOptions);
        forgotEnvelope.Should().NotBeNull();
        forgotEnvelope!.Data.Should().NotBeNull();
        var challengeId = forgotEnvelope.Data!.ChallengeId;

        // 2. Reset password
        var resetRes = await _client.PostAsJsonAsync("/api/auth/reset-password", new
        {
            challengeId,
            code = "123456",
            newPassword = "AdminNewPassword@123"
        });
        resetRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3. Login with new password
        var loginRes = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            password = "AdminNewPassword@123",
            deviceId = "test-device",
            deviceName = "Test Device"
        });
        loginRes.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private sealed record ApiEnvelope<T>(bool Success, string? Message, T? Data);
    private sealed record Paged(List<object> Items, int Page, int PageSize, int TotalCount);
    private sealed record AuthData(string AccessToken);
    private sealed record EnquiryRecord(Guid Id, string FullName, string Email, string InterestedCourse, string PreferredLanguage);
    private sealed record StaffDashboardRecord(int TotalEnquiries, int NewEnquiries, int InFollowUpEnquiries, int EnrolledEnquiries, int OpenTickets, int UnreadNotifications);
    private sealed record MobileChallengeRecord(string ChallengeId, string ExpiresAt);
    private sealed record MobileSessionRecord(string AccessToken, string RefreshToken, MobileStudentRecord Student);
    private sealed record MobileStudentRecord(string Id, string DisplayName, string Email, string PreferredLanguage);
    private sealed record MobileDashboardRecord(MobileStudentRecord Student, double WeeklyGoalMinutes, double MinutesStudiedThisWeek, double AverageQuizScorePercent, int XpPoints, int CurrentStreakDays, int EnrolledCourseCount, int UnreadNotifications);
    private sealed record MobileCourseRecord(string Id, string Title, string Category, string Level, double ProgressPercent, int CompletedLessons, int TotalLessons, string? AccentColor, string EnrollmentStatus);
}
