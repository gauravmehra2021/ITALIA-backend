using SSeuropa.Domain.Common;
using SSeuropa.Domain.Enums;

namespace SSeuropa.Domain.Entities;

public class Material : EntityBase
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaterialType Type { get; set; } = MaterialType.Pdf;
    public string StorageKey { get; set; } = string.Empty;
    public bool IsProtected { get; set; } = true;
    public int SortOrder { get; set; }
}
