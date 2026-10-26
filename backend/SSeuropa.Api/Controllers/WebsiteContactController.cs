using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Services;

namespace SSeuropa.Api.Controllers;

[ApiController]
[Route("api/user")]
public class WebsiteContactController(EnquiryService enquiryService, ILogger<WebsiteContactController> logger) : ControllerBase
{
    private static readonly string[] Subjects =
    [
        "Book an Appointment",
        "Request Information",
        "Document Assistance",
        "Application Status",
        "General Inquiry",
        "Other"
    ];

    private static readonly string[] Services =
    [
        "Immigration Services",
        "CAF & Patronato",
        "Training & Courses",
        "Business Consultancy",
        "Insurance",
        "Indian Consulate",
        "International Visas",
        "Other Services"
    ];

    private static readonly string[] ContactMethods =
    [
        "Email",
        "Phone",
        "WhatsApp",
        "Visit in person"
    ];

    [HttpPost("contact-us")]
    [AllowAnonymous]
    public async Task<IActionResult> SubmitContact([FromBody] WebsiteContactUsRequest request, CancellationToken ct)
    {
        var subject = (request.ObjectType >= 0 && request.ObjectType < Subjects.Length)
            ? Subjects[request.ObjectType]
            : "General Inquiry";

        var serviceInterest = (request.ServiceOfInterest >= 0 && request.ServiceOfInterest < Services.Length)
            ? Services[request.ServiceOfInterest]
            : "General";

        var contactMethod = (request.PreferredContactMethod >= 0 && request.PreferredContactMethod < ContactMethods.Length)
            ? ContactMethods[request.PreferredContactMethod]
            : "Email";

        var fullName = $"{request.Name?.Trim()} {request.Surname?.Trim()}".Trim();
        if (string.IsNullOrWhiteSpace(fullName)) fullName = "Website Visitor";

        var fullPhone = string.IsNullOrWhiteSpace(request.Country)
            ? request.TelephoneNumber ?? string.Empty
            : $"[{request.Country}] {request.TelephoneNumber}".Trim();

        var messageDetail = $"[Subject: {subject}] [Preferred Contact: {contactMethod}] {request.Message?.Trim()}".Trim();

        var enquiryDto = await enquiryService.SubmitAsync(new CreateEnquiryRequest(
            FullName: fullName,
            Email: request.Email?.Trim().ToLowerInvariant() ?? string.Empty,
            Phone: fullPhone,
            InterestedCourse: serviceInterest,
            PreferredLanguage: "Italian",
            Message: messageDetail
        ), ct);

        logger.LogInformation("Contact enquiry submitted from website: {Email} ({FullName}) -> {Service}", enquiryDto.Email, enquiryDto.FullName, serviceInterest);

        return Ok(new
        {
            success = true,
            message = "Thank you! Your contact request has been received. Our team will get back to you shortly.",
            id = enquiryDto.Id
        });
    }

    [HttpPost("visitor")]
    [AllowAnonymous]
    public IActionResult TrackVisitor([FromBody] WebsiteVisitorRequest request)
    {
        logger.LogInformation("Website visitor tracked: Page={Page}, Lang={Lang}, Timezone={Tz}", request.Page, request.Language, request.Timezone);
        return Ok(new { success = true });
    }
}

public record WebsiteContactUsRequest(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("surname")] string? Surname,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("telephone_number")] string? TelephoneNumber,
    [property: JsonPropertyName("country")] string? Country,
    [property: JsonPropertyName("object_type")] int ObjectType,
    [property: JsonPropertyName("service_of_interest")] int ServiceOfInterest,
    [property: JsonPropertyName("preferred_contact_method")] int PreferredContactMethod,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("consent_for_contact")] int ConsentForContact,
    [property: JsonPropertyName("consent_share_data_for_advertising")] int ConsentShareDataForAdvertising,
    [property: JsonPropertyName("consent_third_party_promotional")] int ConsentThirdPartyPromotional);

public record WebsiteVisitorRequest(
    [property: JsonPropertyName("timezone")] string? Timezone,
    [property: JsonPropertyName("language")] string? Language,
    [property: JsonPropertyName("page")] string? Page);
