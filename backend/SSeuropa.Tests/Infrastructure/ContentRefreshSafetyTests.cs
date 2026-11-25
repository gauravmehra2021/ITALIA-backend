using System.Text.Encodings.Web;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Infrastructure.Persistence;

namespace SSeuropa.Tests.Infrastructure;

/// <summary>The startup content fixes must never touch lessons that need nothing, nor lose data.</summary>
public class ContentRefreshSafetyTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Restart_LeavesUpToDateLessonsAlone_AndALessonCopyKeepsEveryField()
    {
        var app = factory.WithWebHostBuilder(b => b.UseSetting("Database:Provider", "InMemory"));
        _ = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var courses = scope.ServiceProvider.GetRequiredService<ICourseRepository>();
        var curriculum = scope.ServiceProvider.GetRequiredService<ICurriculumRepository>();
        var seeder = app.Services.GetServices<IHostedService>().OfType<CourseContentSeeder>().Single();

        var lessons = new List<Lesson>();
        foreach (var c in await courses.GetAllAsync()) lessons.AddRange(await curriculum.GetLessonsByCourseAsync(c.Id));
        lessons.Should().HaveCountGreaterThan(300);
        var stamps = lessons.ToDictionary(l => l.Id, l => l.UpdatedAtUtc);

        await seeder.StartAsync(CancellationToken.None);
        lessons.Should().OnlyContain(l => l.UpdatedAtUtc == stamps[l.Id], "a restart must not rewrite lessons that are already up to date");

        var json = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        foreach (var l in lessons)
        {
            var copy = JsonSerializer.Deserialize<Lesson>(JsonSerializer.Serialize(l, json), json)!;
            copy.Should().BeEquivalentTo(l, o => o.WithStrictOrdering());
        }
    }
}
