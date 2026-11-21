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
        course.SeededUnitKeys.Should().BeEquivalentTo("capitolo-1", "capitolo-2");
        (await curriculum.GetUnitsAsync(course.Id)).Should().HaveCount(2);

        // 1. A plain restart changes nothing.
        await seeder.StartAsync(CancellationToken.None);
        (await CopiesAsync()).Should().Be(1);
        (await curriculum.GetUnitsAsync(course.Id)).Should().HaveCount(2);

        // 2. A course loaded before unit keys existed: the current seed becomes the baseline, nothing is added.
        course.SeededUnitKeys = [];
        await courses.UpdateAsync(course);
        await seeder.StartAsync(CancellationToken.None);
        (await curriculum.GetUnitsAsync(course.Id)).Should().HaveCount(2);
        (await courses.GetByIdAsync(course.Id))!.SeededUnitKeys.Should().BeEquivalentTo("capitolo-1", "capitolo-2");

        // 3. A chapter that is in the seed but not yet in the course is appended once, at the end.
        course = (await courses.GetByIdAsync(course.Id))!;
        course.SeededUnitKeys.Remove("capitolo-2");
        await courses.UpdateAsync(course);
        await seeder.StartAsync(CancellationToken.None);
        var units = await curriculum.GetUnitsAsync(course.Id);
        units.Should().HaveCount(3);
        units.Max(u => u.Order).Should().Be(3);
        await seeder.StartAsync(CancellationToken.None);
        (await curriculum.GetUnitsAsync(course.Id)).Should().HaveCount(3);

        // 4. A unit deleted by an admin is not loaded again.
        await curriculum.DeleteUnitAsync(units.OrderBy(u => u.Order).Last().Id);
        await seeder.StartAsync(CancellationToken.None);
        (await curriculum.GetUnitsAsync(course.Id)).Should().HaveCount(2);
        (await CopiesAsync()).Should().Be(1);
    }
}
