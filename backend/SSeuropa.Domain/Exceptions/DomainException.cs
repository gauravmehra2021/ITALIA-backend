namespace SSeuropa.Domain.Exceptions;

public class DomainException : Exception
{
    public string Code { get; }

    public DomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string resource, Guid id)
        : base("not_found", $"{resource} with id '{id}' was not found.")
    {
    }

    public NotFoundException(string message)
        : base("not_found", message)
    {
    }
}

public class UnauthorizedAccessDomainException : DomainException
{
    public UnauthorizedAccessDomainException(string message = "Access denied.")
        : base("unauthorized", message)
    {
    }
}

public class ValidationDomainException : DomainException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationDomainException(string message, IDictionary<string, string[]>? errors = null)
        : base("validation_error", message)
    {
        Errors = errors is null
            ? new Dictionary<string, string[]>()
            : new Dictionary<string, string[]>(errors);
    }
}

public class ConflictException : DomainException
{
    public ConflictException(string message)
        : base("conflict", message)
    {
    }
}
