using System.Net.Mail;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Services;
using SSeuropa.Domain.Exceptions;

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
        var errors = ValidateRequest(request);
        if (errors.Count > 0)
        {
            throw new ValidationDomainException("Validation failed.", errors);
        }

        var subject = (request.ObjectType >= 0 && request.ObjectType < Subjects.Length)
            ? Subjects[request.ObjectType]
            : "General Inquiry";

        var serviceInterest = ResolveServices(request);

        var contactMethod = (request.PreferredContactMethod >= 0 && request.PreferredContactMethod < ContactMethods.Length)
            ? ContactMethods[request.PreferredContactMethod]
            : "Email";

        var fullName = $"{request.Name?.Trim()} {request.Surname?.Trim()}".Trim();
        if (string.IsNullOrWhiteSpace(fullName))
        {
            fullName = "Website Visitor";
        }
        if (fullName.Length > 120)
        {
            fullName = fullName[..120];
        }

        var cleanPhone = request.TelephoneNumber?.Trim() ?? string.Empty;
        var cleanCountry = request.Country?.Trim() ?? string.Empty;
        var fullPhone = string.IsNullOrWhiteSpace(cleanCountry)
            ? (string.IsNullOrWhiteSpace(cleanPhone) ? "-" : cleanPhone)
            : (string.IsNullOrWhiteSpace(cleanPhone) ? $"[{cleanCountry}]" : $"[{cleanCountry}] {cleanPhone}");
        if (fullPhone.Length > 40)
        {
            fullPhone = fullPhone[..40];
        }

        var preferredLanguage = string.IsNullOrWhiteSpace(request.PreferredLanguage)
            ? "Italian"
            : request.PreferredLanguage.Trim();
        if (preferredLanguage.Length > 40)
        {
            preferredLanguage = preferredLanguage[..40];
        }

        var consentSummary = $"Marketing:{(request.ConsentShareDataForAdvertising == 1 ? "Yes" : "No")}, ThirdParty:{(request.ConsentThirdPartyPromotional == 1 ? "Yes" : "No")}";
        var messageDetail = $"[Subject: {subject}] [Preferred Contact: {contactMethod}] [{consentSummary}] {request.Message?.Trim()}".Trim();
        if (messageDetail.Length > 2000)
        {
            messageDetail = messageDetail[..2000];
        }

        var enquiryDto = await enquiryService.SubmitAsync(new CreateEnquiryRequest(
            FullName: fullName,
            Email: request.Email!.Trim().ToLowerInvariant(),
            Phone: fullPhone,
            InterestedCourse: serviceInterest,
            PreferredLanguage: preferredLanguage,
            Message: messageDetail
        ), ct);

        logger.LogInformation(
            "Contact enquiry submitted from website: {Email} ({FullName}) -> {Service} [Lang={Lang}]",
            enquiryDto.Email,
            enquiryDto.FullName,
            serviceInterest,
            preferredLanguage);

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

    private static Dictionary<string, string[]> ValidateRequest(WebsiteContactUsRequest request)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors["name"] = ["First name is required."];
        }

        var email = request.Email?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(email) || !MailAddress.TryCreate(email, out var parsed) || parsed.Address != email || email.Length > 160)
        {
            errors["email"] = ["A valid email address is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            errors["message"] = ["Message is required."];
        }
        else if (request.Message.Trim().Length > 1800)
        {
            errors["message"] = ["Message must be 1800 characters or fewer."];
        }

        if (request.ConsentForContact != 1)
        {
            errors["consent_for_contact"] = ["Privacy and contact consent is required."];
        }

        return errors;
    }

    private static string ResolveServices(WebsiteContactUsRequest request)
    {
        if (request.ServicesOfInterest is { Length: > 0 })
        {
            var resolved = request.ServicesOfInterest
                .Where(idx => idx >= 0 && idx < Services.Length)
                .Select(idx => Services[idx])
                .Distinct()
                .ToList();

            if (resolved.Count > 0)
            {
                var joined = string.Join(", ", resolved);
                return joined.Length <= 120 ? joined : joined[..120];
            }
        }

        return (request.ServiceOfInterest >= 0 && request.ServiceOfInterest < Services.Length)
            ? Services[request.ServiceOfInterest]
            : "General";
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
    [property: JsonPropertyName("consent_third_party_promotional")] int ConsentThirdPartyPromotional,
    [property: JsonPropertyName("services_of_interest")] int[]? ServicesOfInterest = null,
    [property: JsonPropertyName("preferred_language")] string? PreferredLanguage = null);

public record WebsiteVisitorRequest(
    [property: JsonPropertyName("timezone")] string? Timezone,
    [property: JsonPropertyName("language")] string? Language,
    [property: JsonPropertyName("page")] string? Page);
