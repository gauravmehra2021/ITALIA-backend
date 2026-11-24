using FluentAssertions;
using SSeuropa.Application.Services;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Infrastructure.Persistence;
using SSeuropa.Infrastructure.Repositories;

namespace SSeuropa.Tests.Application;

/// <summary>The startup step that organises old content into modules runs on every start: it must change nothing the second time.</summary>
public class ModuleStructureTests
{
    [Fact]
    public async Task OldUnit_IsGroupedOnce_ByClassroomLesson_AndProgressIsKept()
    {
        var store = new InMemoryStore();
        var repo = new InMemoryCurriculumRepository(store);
        var courseId = Guid.NewGuid();
        var unit = new CourseUnit { CourseId = courseId, Order = 1, Title = "Chapter 1", Status = LessonStatus.Published };
        await repo.AddUnitAsync(unit);

        string[] titles = ["Lesson 1 · Alphabet", "Lesson 1 · C and G", "Lesson 2 · Essere", "Lesson 2 · Nationalities"];
        var lessons = titles.Select((t, i) => new Lesson { CourseId = courseId, UnitId = unit.Id, Order = i + 1, Title = t, Status = LessonStatus.Published }).ToList();
        lessons.Add(new Lesson { CourseId = courseId, UnitId = unit.Id, Order = 5, Title = "Chapter 1 test", Kind = LessonKind.Exam, Status = LessonStatus.Published });
        foreach (var l in lessons) await repo.AddLessonAsync(l);
        var userId = Guid.NewGuid();
        await repo.UpsertProgressAsync(new LessonProgress { UserId = userId, CourseId = courseId, UnitId = unit.Id, LessonId = lessons[0].Id, Completed = true, Stars = 3 });

        (await ModuleStructure.EnsureCourseAsync(repo, courseId)).Should().BeGreaterThan(0);
        (await ModuleStructure.EnsureCourseAsync(repo, courseId)).Should().Be(0, "a second run must not change anything");

        var modules = await repo.GetModulesByUnitAsync(unit.Id);
        modules.Select(m => m.Title).Should().Equal("Lezione 1", "Lezione 2", "Chapter 1 test");
        modules[0].Description.Should().Be("Alphabet · C and G");

        var ordered = (await repo.GetLessonsByUnitAsync(unit.Id)).OrderBy(l => l.Order).ToList();
        ordered.Should().HaveCount(7); // 5 lessons + a video for each lesson module (the test module has none)
        ordered.Select(l => l.IsModuleVideo).Should().Equal(true, false, false, true, false, false, false);
        modules.Select(m => m.IsTest).Should().Equal(false, false, true);
        ordered.Where(l => !l.IsModuleVideo).Select(l => l.Title).Should().Equal(titles.Append("Chapter 1 test"));
        (await repo.GetProgressAsync(userId, courseId)).Should().ContainSingle(p => p.LessonId == lessons[0].Id && p.Completed);
    }

    [Fact]
    public async Task ChapterTestModule_MadeEarlierWithAVideoPlaceholder_LosesThePlaceholder()
    {
        // The state of a database organised by the first version (the test module got a video slot).
        var store = new InMemoryStore();
        var repo = new InMemoryCurriculumRepository(store);
        var courseId = Guid.NewGuid();
        var unit = new CourseUnit { CourseId = courseId, Order = 1, Title = "Chapter 2", Status = LessonStatus.Published };
        await repo.AddUnitAsync(unit);
        var module = new CourseModule
        {
            CourseId = courseId, UnitId = unit.Id, Order = 1, Title = "Chapter 2 test",
            Description = "Chapter test: pass with 60% to finish the chapter.",
        };
        await repo.AddModuleAsync(module);
        await repo.AddLessonAsync(ModuleStructure.NewVideoLesson(module, LessonStatus.Published));
        await repo.AddLessonAsync(new Lesson { CourseId = courseId, UnitId = unit.Id, ModuleId = module.Id, Order = 2, Title = "Chapter 2 test", Kind = LessonKind.Exam, Status = LessonStatus.Published });

        await ModuleStructure.EnsureCourseAsync(repo, courseId);
        (await ModuleStructure.EnsureCourseAsync(repo, courseId)).Should().Be(0);

        (await repo.GetModuleAsync(module.Id))!.IsTest.Should().BeTrue();
        (await repo.GetLessonsByUnitAsync(unit.Id)).Should().ContainSingle().Which.Kind.Should().Be(LessonKind.Exam);
    }
}
