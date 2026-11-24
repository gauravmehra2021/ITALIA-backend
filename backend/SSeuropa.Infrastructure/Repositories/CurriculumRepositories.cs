using MongoDB.Driver;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Infrastructure.Persistence;

namespace SSeuropa.Infrastructure.Repositories;

public class MongoCurriculumRepository(MongoDbContext db) : ICurriculumRepository
{
    public async Task<IReadOnlyList<CourseUnit>> GetUnitsAsync(Guid courseId, CancellationToken ct = default) =>
        await db.CourseUnits.Find(u => u.CourseId == courseId).SortBy(u => u.Order).ToListAsync(ct);

    public async Task<CourseUnit?> GetUnitAsync(Guid id, CancellationToken ct = default) =>
        await db.CourseUnits.Find(u => u.Id == id).FirstOrDefaultAsync(ct);

    public Task AddUnitAsync(CourseUnit unit, CancellationToken ct = default) =>
        db.CourseUnits.InsertOneAsync(unit, cancellationToken: ct);

    public Task UpdateUnitAsync(CourseUnit unit, CancellationToken ct = default) =>
        MongoRepoHelpers.ReplaceAsync(db.CourseUnits, unit, ct);

    public async Task DeleteUnitAsync(Guid id, CancellationToken ct = default)
    {
        await db.Lessons.DeleteManyAsync(l => l.UnitId == id, ct);
        await db.CourseModules.DeleteManyAsync(m => m.UnitId == id, ct);
        await db.LessonProgress.DeleteManyAsync(p => p.UnitId == id, ct);
        await db.CourseUnits.DeleteOneAsync(u => u.Id == id, ct);
    }

    public async Task<IReadOnlyList<CourseModule>> GetModulesAsync(Guid courseId, CancellationToken ct = default) =>
        await db.CourseModules.Find(m => m.CourseId == courseId).SortBy(m => m.Order).ToListAsync(ct);

    public async Task<IReadOnlyList<CourseModule>> GetModulesByUnitAsync(Guid unitId, CancellationToken ct = default) =>
        await db.CourseModules.Find(m => m.UnitId == unitId).SortBy(m => m.Order).ToListAsync(ct);

    public async Task<CourseModule?> GetModuleAsync(Guid id, CancellationToken ct = default) =>
        await db.CourseModules.Find(m => m.Id == id).FirstOrDefaultAsync(ct);

    public Task AddModuleAsync(CourseModule module, CancellationToken ct = default) =>
        db.CourseModules.InsertOneAsync(module, cancellationToken: ct);

    public Task UpdateModuleAsync(CourseModule module, CancellationToken ct = default) =>
        MongoRepoHelpers.ReplaceAsync(db.CourseModules, module, ct);

    public async Task DeleteModuleAsync(Guid id, CancellationToken ct = default)
    {
        var lessonIds = await db.Lessons.Find(l => l.ModuleId == id).Project(l => l.Id).ToListAsync(ct);
        await db.LessonProgress.DeleteManyAsync(p => lessonIds.Contains(p.LessonId), ct);
        await db.Lessons.DeleteManyAsync(l => l.ModuleId == id, ct);
        await db.CourseModules.DeleteOneAsync(m => m.Id == id, ct);
    }

    public async Task<IReadOnlyList<Lesson>> GetLessonsByCourseAsync(Guid courseId, CancellationToken ct = default) =>
        await db.Lessons.Find(l => l.CourseId == courseId).SortBy(l => l.Order).ToListAsync(ct);

    public async Task<IReadOnlyList<Lesson>> GetLessonsByUnitAsync(Guid unitId, CancellationToken ct = default) =>
        await db.Lessons.Find(l => l.UnitId == unitId).SortBy(l => l.Order).ToListAsync(ct);

    public async Task<Lesson?> GetLessonAsync(Guid id, CancellationToken ct = default) =>
        await db.Lessons.Find(l => l.Id == id).FirstOrDefaultAsync(ct);

    public Task AddLessonAsync(Lesson lesson, CancellationToken ct = default) =>
        db.Lessons.InsertOneAsync(lesson, cancellationToken: ct);

    public Task UpdateLessonAsync(Lesson lesson, CancellationToken ct = default) =>
        MongoRepoHelpers.ReplaceAsync(db.Lessons, lesson, ct);

    public async Task DeleteLessonAsync(Guid id, CancellationToken ct = default)
    {
        await db.LessonProgress.DeleteManyAsync(p => p.LessonId == id, ct);
        await db.Lessons.DeleteOneAsync(l => l.Id == id, ct);
    }

    public async Task<IReadOnlyList<LessonProgress>> GetProgressAsync(Guid userId, Guid courseId, CancellationToken ct = default) =>
        await db.LessonProgress.Find(p => p.UserId == userId && p.CourseId == courseId).ToListAsync(ct);

    public async Task<IReadOnlyList<LessonProgress>> GetAllProgressForUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.LessonProgress.Find(p => p.UserId == userId).ToListAsync(ct);

    public async Task<IReadOnlyList<LessonProgress>> GetProgressForCourseAsync(Guid courseId, CancellationToken ct = default) =>
        await db.LessonProgress.Find(p => p.CourseId == courseId).ToListAsync(ct);

    public async Task<LessonProgress?> GetLessonProgressAsync(Guid userId, Guid lessonId, CancellationToken ct = default) =>
        await db.LessonProgress.Find(p => p.UserId == userId && p.LessonId == lessonId).FirstOrDefaultAsync(ct);

    public Task UpsertProgressAsync(LessonProgress progress, CancellationToken ct = default)
    {
        progress.UpdatedAtUtc = DateTime.UtcNow;
        return db.LessonProgress.ReplaceOneAsync(
            p => p.Id == progress.Id,
            progress,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    public async Task DeleteCourseCurriculumAsync(Guid courseId, CancellationToken ct = default)
    {
        await db.LessonProgress.DeleteManyAsync(p => p.CourseId == courseId, ct);
        await db.Lessons.DeleteManyAsync(l => l.CourseId == courseId, ct);
        await db.CourseUnits.DeleteManyAsync(u => u.CourseId == courseId, ct);
        await db.CourseModules.DeleteManyAsync(m => m.CourseId == courseId, ct);
    }
}

public class InMemoryCurriculumRepository(InMemoryStore store) : ICurriculumRepository
{
    private readonly object _gate = new();

    private Task<IReadOnlyList<T>> List<T>(IEnumerable<T> items)
    {
        lock (_gate) return Task.FromResult<IReadOnlyList<T>>(items.ToList());
    }

    public Task<IReadOnlyList<CourseUnit>> GetUnitsAsync(Guid courseId, CancellationToken ct = default) =>
        List(store.CourseUnits.Where(u => u.CourseId == courseId).OrderBy(u => u.Order));

    public Task<CourseUnit?> GetUnitAsync(Guid id, CancellationToken ct = default)
    {
        lock (_gate) return Task.FromResult(store.CourseUnits.FirstOrDefault(u => u.Id == id));
    }

    public Task AddUnitAsync(CourseUnit unit, CancellationToken ct = default)
    {
        lock (_gate) store.CourseUnits.Add(unit);
        return Task.CompletedTask;
    }

    public Task UpdateUnitAsync(CourseUnit unit, CancellationToken ct = default)
    {
        unit.UpdatedAtUtc = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task DeleteUnitAsync(Guid id, CancellationToken ct = default)
    {
        lock (_gate)
        {
            store.Lessons.RemoveAll(l => l.UnitId == id);
            store.CourseModules.RemoveAll(m => m.UnitId == id);
            store.LessonProgress.RemoveAll(p => p.UnitId == id);
            store.CourseUnits.RemoveAll(u => u.Id == id);
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<CourseModule>> GetModulesAsync(Guid courseId, CancellationToken ct = default) =>
        List(store.CourseModules.Where(m => m.CourseId == courseId).OrderBy(m => m.Order));

    public Task<IReadOnlyList<CourseModule>> GetModulesByUnitAsync(Guid unitId, CancellationToken ct = default) =>
        List(store.CourseModules.Where(m => m.UnitId == unitId).OrderBy(m => m.Order));

    public Task<CourseModule?> GetModuleAsync(Guid id, CancellationToken ct = default)
    {
        lock (_gate) return Task.FromResult(store.CourseModules.FirstOrDefault(m => m.Id == id));
    }

    public Task AddModuleAsync(CourseModule module, CancellationToken ct = default)
    {
        lock (_gate) store.CourseModules.Add(module);
        return Task.CompletedTask;
    }

    public Task UpdateModuleAsync(CourseModule module, CancellationToken ct = default)
    {
        module.UpdatedAtUtc = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task DeleteModuleAsync(Guid id, CancellationToken ct = default)
    {
        lock (_gate)
        {
            var lessonIds = store.Lessons.Where(l => l.ModuleId == id).Select(l => l.Id).ToHashSet();
            store.LessonProgress.RemoveAll(p => lessonIds.Contains(p.LessonId));
            store.Lessons.RemoveAll(l => l.ModuleId == id);
            store.CourseModules.RemoveAll(m => m.Id == id);
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Lesson>> GetLessonsByCourseAsync(Guid courseId, CancellationToken ct = default) =>
        List(store.Lessons.Where(l => l.CourseId == courseId).OrderBy(l => l.Order));

    public Task<IReadOnlyList<Lesson>> GetLessonsByUnitAsync(Guid unitId, CancellationToken ct = default) =>
        List(store.Lessons.Where(l => l.UnitId == unitId).OrderBy(l => l.Order));

    public Task<Lesson?> GetLessonAsync(Guid id, CancellationToken ct = default)
    {
        lock (_gate) return Task.FromResult(store.Lessons.FirstOrDefault(l => l.Id == id));
    }

    public Task AddLessonAsync(Lesson lesson, CancellationToken ct = default)
    {
        lock (_gate) store.Lessons.Add(lesson);
        return Task.CompletedTask;
    }

    public Task UpdateLessonAsync(Lesson lesson, CancellationToken ct = default)
    {
        lesson.UpdatedAtUtc = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public Task DeleteLessonAsync(Guid id, CancellationToken ct = default)
    {
        lock (_gate)
        {
            store.LessonProgress.RemoveAll(p => p.LessonId == id);
            store.Lessons.RemoveAll(l => l.Id == id);
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<LessonProgress>> GetProgressAsync(Guid userId, Guid courseId, CancellationToken ct = default) =>
        List(store.LessonProgress.Where(p => p.UserId == userId && p.CourseId == courseId));

    public Task<IReadOnlyList<LessonProgress>> GetAllProgressForUserAsync(Guid userId, CancellationToken ct = default) =>
        List(store.LessonProgress.Where(p => p.UserId == userId));

    public Task<IReadOnlyList<LessonProgress>> GetProgressForCourseAsync(Guid courseId, CancellationToken ct = default) =>
        List(store.LessonProgress.Where(p => p.CourseId == courseId));

    public Task<LessonProgress?> GetLessonProgressAsync(Guid userId, Guid lessonId, CancellationToken ct = default)
    {
        lock (_gate) return Task.FromResult(store.LessonProgress.FirstOrDefault(p => p.UserId == userId && p.LessonId == lessonId));
    }

    public Task UpsertProgressAsync(LessonProgress progress, CancellationToken ct = default)
    {
        lock (_gate)
        {
            progress.UpdatedAtUtc = DateTime.UtcNow;
            if (!store.LessonProgress.Any(p => p.Id == progress.Id))
                store.LessonProgress.Add(progress);
        }
        return Task.CompletedTask;
    }

    public Task DeleteCourseCurriculumAsync(Guid courseId, CancellationToken ct = default)
    {
        lock (_gate)
        {
            store.LessonProgress.RemoveAll(p => p.CourseId == courseId);
            store.Lessons.RemoveAll(l => l.CourseId == courseId);
            store.CourseUnits.RemoveAll(u => u.CourseId == courseId);
            store.CourseModules.RemoveAll(m => m.CourseId == courseId);
        }
        return Task.CompletedTask;
    }
}
