using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Infrastructure.Persistence;

namespace SSeuropa.Tests.Infrastructure;

/// <summary>
/// Built-in courses are loaded once; later chapters are appended without duplicates; the old
/// one-course syllabus is split into one course per chapter without losing progress.
/// </summary>
public class CourseContentSeederTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private const string OldSlug = "corso-italiano-a0";
    private static string ChapterSlug(int n) => $"corso-italiano-capitolo-{n}";

    private static async Task<Course> CourseAsync(ICourseRepository courses, string slug) =>
        (await courses.GetAllAsync()).Single(c => c.Slug == slug && !c.IsDeleted);

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

        // A fresh install: one course per chapter, one unit each, no old combined course.
        (await courses.GetAllAsync()).Should().NotContain(c => c.Slug == OldSlug);
        for (var n = 1; n <= 12; n++)
        {
            var c = await CourseAsync(courses, ChapterSlug(n));
            c.SeededUnitKeys.Should().Equal($"capitolo-{n}");
            (await curriculum.GetUnitsAsync(c.Id)).Should().ContainSingle();
        }

        // 1. A plain restart changes nothing.
        var before = (await courses.GetAllAsync()).Count;
        await seeder.StartAsync(CancellationToken.None);
        (await courses.GetAllAsync()).Should().HaveCount(before);

        // 2. A unit deleted by an admin is not loaded again.
        var chapter3 = await CourseAsync(courses, ChapterSlug(3));
        await curriculum.DeleteUnitAsync((await curriculum.GetUnitsAsync(chapter3.Id)).Single().Id);
        await seeder.StartAsync(CancellationToken.None);
        (await curriculum.GetUnitsAsync(chapter3.Id)).Should().BeEmpty();

        // 3. A course loaded before unit keys existed: the current seed becomes the baseline, nothing is added.
        chapter3.SeededUnitKeys = [];
        await courses.UpdateAsync(chapter3);
        await seeder.StartAsync(CancellationToken.None);
        (await curriculum.GetUnitsAsync(chapter3.Id)).Should().BeEmpty();
        (await courses.GetByIdAsync(chapter3.Id))!.SeededUnitKeys.Should().Equal("capitolo-3");
    }

    [Fact]
    public async Task LessonsAlreadyLive_GetPlainPunjabi_MissingPictures_AndEnglishTableColumn()
    {
        var app = factory.WithWebHostBuilder(b => b.UseSetting("Database:Provider", "InMemory"));
        _ = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var courses = sp.GetRequiredService<ICourseRepository>();
        var curriculum = sp.GetRequiredService<ICurriculumRepository>();
        var translations = sp.GetRequiredService<ITranslationRepository>();
        var seeder = app.Services.GetServices<IHostedService>().OfType<CourseContentSeeder>().Single();

        // Put back what the live server has: the old spelling and title, no picture, the old table.
        var lessons1 = await curriculum.GetLessonsByCourseAsync((await CourseAsync(courses, ChapterSlug(1))).Id);
        var pronouns = lessons1.Single(l => l.Blocks.Any(b => b.Title == "Il verbo ESSERE (to be)"));
        var essere = pronouns.Blocks.Single(b => b.Title == "Il verbo ESSERE (to be)");
        essere.Title = "Il verbo ESSERE (to be · Hoṇā)";
        essere.Table[1][^1] = "Maiṁ Simran hāṁ.";
        var card = pronouns.Blocks.SelectMany(b => b.Items).Single(i => i.Italian == "Come ti chiami?");
        var picture = card.ImageUrl;
        picture.Should().NotBeNullOrEmpty();
        card.ImageUrl = null;
        card.Punjabi = "Tuhāḍā nām kī hai?";
        await curriculum.UpdateLessonAsync(pronouns);

        var lessons3 = await curriculum.GetLessonsByCourseAsync((await CourseAsync(courses, ChapterSlug(3))).Id);
        var clock = lessons3.Single(l => l.Blocks.Any(b => b.Title == "Le ore e i minuti"));
        var hours = clock.Blocks.Single(b => b.Title == "Le ore e i minuti");
        var seedTable = hours.Table.Select(r => r.ToList()).ToList();
        hours.Table = seedTable.Select(r => r.Where((_, i) => i != 2).ToList()).ToList(); // no English column
        hours.Table[0].Should().Equal("Time", "Italiano", "Punjabi");
        await curriculum.UpdateLessonAsync(clock);

        var row = (await translations.GetAllAsync()).First(t => t.Punjabi is not null);
        row.Punjabi = "Maiṁ";
        await translations.UpdateAsync(row);

        await seeder.StartAsync(CancellationToken.None);

        var fixedLesson = (await curriculum.GetLessonAsync(pronouns.Id))!;
        var fixedBlock = fixedLesson.Blocks.Single(b => b.Id == essere.Id);
        fixedBlock.Title.Should().Be("Il verbo ESSERE (to be)");
        fixedBlock.Table[1][^1].Should().Be("Main Simran haan.");
        var fixedCard = fixedLesson.Blocks.SelectMany(b => b.Items).Single(i => i.Id == card.Id);
        fixedCard.ImageUrl.Should().Be(picture);
        fixedCard.Punjabi.Should().Be("Tuhada naam ki hai?");
        (await curriculum.GetLessonAsync(clock.Id))!.Blocks.Single(b => b.Id == hours.Id).Table.Should().BeEquivalentTo(seedTable, o => o.WithStrictOrdering());
        (await translations.GetAllAsync()).Single(t => t.Id == row.Id).Punjabi.Should().Be("Main");

        // A picture an admin chose is kept.
        fixedCard.ImageUrl = "/uploads/my-own.png";
        await curriculum.UpdateLessonAsync(fixedLesson);
        await seeder.StartAsync(CancellationToken.None);
        (await curriculum.GetLessonAsync(pronouns.Id))!.Blocks.SelectMany(b => b.Items).Single(i => i.Id == card.Id).ImageUrl.Should().Be("/uploads/my-own.png");
    }

    [Fact]
    public async Task OldOneCourseSyllabus_IsSplitIntoChapterCourses_KeepingProgressEnrolmentsAndBatches()
    {
        var app = factory.WithWebHostBuilder(b => b.UseSetting("Database:Provider", "InMemory"));
        _ = app.CreateClient();
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var courses = sp.GetRequiredService<ICourseRepository>();
        var curriculum = sp.GetRequiredService<ICurriculumRepository>();
        var enrollments = sp.GetRequiredService<IEnrollmentRepository>();
        var batches = sp.GetRequiredService<IBatchRepository>();
        var seeder = app.Services.GetServices<IHostedService>().OfType<CourseContentSeeder>().Single();

        // Rebuild the live situation: one course "corso-italiano-a0" that has only Capitolo 1–2
        // (the second renamed by an admin), a student with progress in it, and a batch.
        var old = new Course
        {
            Title = "Corso di Italiano — Livello A0", Slug = OldSlug, Status = CourseStatus.Published,
            MentorId = Guid.NewGuid(), SeededUnitKeys = ["capitolo-1", "capitolo-2"],
        };
        await courses.AddAsync(old);
        var moved = new List<CourseUnit>();
        for (var n = 1; n <= 2; n++)
        {
            var chapter = await CourseAsync(courses, ChapterSlug(n));
            var unit = (await curriculum.GetUnitsAsync(chapter.Id)).Single();
            foreach (var m in await curriculum.GetModulesByUnitAsync(unit.Id)) { m.CourseId = old.Id; await curriculum.UpdateModuleAsync(m); }
            foreach (var l in await curriculum.GetLessonsByUnitAsync(unit.Id)) { l.CourseId = old.Id; await curriculum.UpdateLessonAsync(l); }
            unit.CourseId = old.Id;
            unit.Order = n;
            if (n == 2) { unit.Title = "Il mio capitolo"; unit.TitleIt = "Il mio capitolo"; }
            await curriculum.UpdateUnitAsync(unit);
            moved.Add(unit);
        }
        foreach (var c in (await courses.GetAllAsync()).Where(c => c.Slug.StartsWith("corso-italiano-capitolo-")))
        {
            await curriculum.DeleteCourseCurriculumAsync(c.Id);
            await courses.DeleteAsync(c.Id);
        }

        var studentId = Guid.NewGuid();
        await enrollments.AddAsync(new Enrollment { UserId = studentId, CourseId = old.Id, Status = EnrollmentStatus.Active, ActivatedAtUtc = DateTime.UtcNow });
        var chapter1Lessons = (await curriculum.GetLessonsByUnitAsync(moved[0].Id)).Select(l => l.Id).ToList();
        await curriculum.UpsertProgressAsync(new LessonProgress
        {
            UserId = studentId, CourseId = old.Id, UnitId = moved[0].Id, LessonId = chapter1Lessons[1], Completed = true, Stars = 3,
        });
        var other = Guid.NewGuid();
        var batch = await batches.AddAsync(new Batch { Name = "A0 Morning", CourseIds = [other, old.Id] });
        var a1 = await CourseAsync(courses, "italian-a1-foundations");
        a1.DisplayOrder = 1; // its place on the live server
        await courses.UpdateAsync(a1);

        // Deploy: the seeder splits the old course.
        await seeder.StartAsync(CancellationToken.None);

        (await courses.GetAllAsync()).Should().NotContain(c => c.Slug == OldSlug);
        var chapters = new List<Course>();
        for (var n = 1; n <= 12; n++) chapters.Add(await CourseAsync(courses, ChapterSlug(n)));

        // Chapter 1–2 are the same units and lessons (ids kept), now in their own courses.
        var unit1 = (await curriculum.GetUnitsAsync(chapters[0].Id)).Should().ContainSingle().Subject;
        unit1.Id.Should().Be(moved[0].Id);
        unit1.Order.Should().Be(1);
        (await curriculum.GetLessonsByUnitAsync(unit1.Id)).Select(l => l.Id).Should().Equal(chapter1Lessons);
        (await curriculum.GetLessonsByCourseAsync(chapters[0].Id)).Should().HaveCount(chapter1Lessons.Count);
        (await curriculum.GetModulesAsync(chapters[0].Id)).Should().NotBeEmpty();
        (await curriculum.GetUnitsAsync(chapters[1].Id)).Single().Id.Should().Be(moved[1].Id); // renamed: matched by position
        chapters[0].MentorId.Should().Be(old.MentorId);

        // Chapters 3–12 were never in the old course: loaded fresh.
        (await curriculum.GetLessonsByCourseAsync(chapters[2].Id)).Should().NotBeEmpty();

        // The student keeps the progress and can open every chapter.
        var progress = await curriculum.GetProgressAsync(studentId, chapters[0].Id);
        progress.Should().ContainSingle().Which.LessonId.Should().Be(chapter1Lessons[1]);
        var mine = await enrollments.GetByUserAsync(studentId);
        mine.Select(e => e.CourseId).Should().BeEquivalentTo(chapters.Select(c => c.Id));
        mine.Should().OnlyContain(e => e.Status == EnrollmentStatus.Active);

        // The batch has all chapters where the old course was.
        (await batches.GetByIdAsync(batch.Id))!.CourseIds.Should().Equal([other, .. chapters.Select(c => c.Id)]);

        // Chapter courses come first in the catalogue.
        (await CourseAsync(courses, "italian-a1-foundations")).DisplayOrder.Should().BeGreaterThan(chapters.Max(c => c.DisplayOrder));

        // A restart does nothing more.
        var count = (await courses.GetAllAsync()).Count;
        await seeder.StartAsync(CancellationToken.None);
        (await courses.GetAllAsync()).Should().HaveCount(count);
        (await enrollments.GetByUserAsync(studentId)).Should().HaveCount(12);
        (await curriculum.GetUnitsAsync(chapters[0].Id)).Should().ContainSingle();
    }
}
