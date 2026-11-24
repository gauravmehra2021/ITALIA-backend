using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SSeuropa.Application.Interfaces;
using SSeuropa.Infrastructure.Persistence;

namespace SSeuropa.Tests.Infrastructure;

/// <summary>Built-in courses are loaded once; later chapters are appended without duplicates.</summary>
public class CourseContentSeederTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Slug = "corso-italiano-a0";
    private static readonly string[] AllChapters = Enumerable.Range(1, 12).Select(n => $"capitolo-{n}").ToArray();
    private const int ChapterCount = 12;

    [Fact]
    public async Task Restart_NeverDuplicates_AppendsOnlyNewChapters_AndRespectsDeletedUnits()
    {
        var app = factory.WithWebHostBuilder(b => b.UseSetting("Database:Provider", "InMemory"));
        _ = app.CreateClient(); // starts the host, which runs the seeder once
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var courses = sp.GetRequiredService<ICourseRepository>();
        var curriculum = sp.GetRequiredService<ICurriculumRepository>();
        var seeder = app.Services.GetServices<IHostedService>().OfType<CourseContentSeeder>().Single();

        async Task<int> CopiesAsync() => (await courses.GetAllAsync()).Count(c => c.Slug == Slug);
        var course = (await courses.GetAllAsync()).Single(c => c.Slug == Slug);
        course.SeededUnitKeys.Should().BeEquivalentTo(AllChapters);
        (await curriculum.GetUnitsAsync(course.Id)).Should().HaveCount(ChapterCount);

        // 1. A plain restart changes nothing.
        await seeder.StartAsync(CancellationToken.None);
        (await CopiesAsync()).Should().Be(1);
        (await curriculum.GetUnitsAsync(course.Id)).Should().HaveCount(ChapterCount);

        // 2. The live situation: the course was loaded when only chapters 1–2 existed.
        var units = (await curriculum.GetUnitsAsync(course.Id)).OrderBy(u => u.Order).ToList();
        foreach (var later in units.Skip(2)) await curriculum.DeleteUnitAsync(later.Id);
        course.SeededUnitKeys = ["capitolo-1", "capitolo-2"];
        var builtInTitle = course.Title;
        course.Title = "Corso di Italiano — Livello A0"; // the title it had on the live server
        await courses.UpdateAsync(course);
        var chapter1LessonIds = (await curriculum.GetLessonsByUnitAsync(units[0].Id)).Select(l => l.Id).ToList();

        // Deploying the new seed appends chapters 3–12 once, after the existing ones,
        // and leaves chapters 1–2 (and their lesson ids, so student progress) untouched.
        await seeder.StartAsync(CancellationToken.None);
        units = (await curriculum.GetUnitsAsync(course.Id)).OrderBy(u => u.Order).ToList();
        units.Should().HaveCount(ChapterCount);
        units.Select(u => u.Order).Should().Equal(Enumerable.Range(1, ChapterCount));
        units[2].Title.Should().StartWith("Chapter 3");
        units[11].Title.Should().StartWith("Chapter 12");
        (await curriculum.GetLessonsByUnitAsync(units[0].Id)).Select(l => l.Id).Should().Equal(chapter1LessonIds);
        var refreshed = (await courses.GetByIdAsync(course.Id))!;
        refreshed.SeededUnitKeys.Should().BeEquivalentTo(AllChapters);
        refreshed.Title.Should().Be(builtInTitle); // old built-in title → refreshed with the new chapters
        refreshed.CefrLevel.Should().Be("A0–A2");

        // An admin rename is kept when later chapters arrive.
        refreshed.Title = "Il nostro corso";
        refreshed.SeededUnitKeys.Remove("capitolo-12");
        await courses.UpdateAsync(refreshed);
        await curriculum.DeleteUnitAsync(units[11].Id);
        await seeder.StartAsync(CancellationToken.None);
        (await courses.GetByIdAsync(course.Id))!.Title.Should().Be("Il nostro corso");
        units = (await curriculum.GetUnitsAsync(course.Id)).OrderBy(u => u.Order).ToList();

        await seeder.StartAsync(CancellationToken.None);
        (await curriculum.GetUnitsAsync(course.Id)).Should().HaveCount(ChapterCount);

        // 3. A course loaded before unit keys existed: the current seed becomes the baseline, nothing is added.
        course = (await courses.GetByIdAsync(course.Id))!;
        course.SeededUnitKeys = [];
        await courses.UpdateAsync(course);
        await seeder.StartAsync(CancellationToken.None);
        (await curriculum.GetUnitsAsync(course.Id)).Should().HaveCount(ChapterCount);
        (await courses.GetByIdAsync(course.Id))!.SeededUnitKeys.Should().BeEquivalentTo(AllChapters);

        // 4. A unit deleted by an admin is not loaded again.
        await curriculum.DeleteUnitAsync(units[^1].Id);
        await seeder.StartAsync(CancellationToken.None);
        (await curriculum.GetUnitsAsync(course.Id)).Should().HaveCount(ChapterCount - 1);
        (await CopiesAsync()).Should().Be(1);
    }
}
