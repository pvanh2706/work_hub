namespace WorkHub.Shared.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, Guid id)
        : base($"{entityName} with id '{id}' was not found.") { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Access denied.")
        : base(message) { }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
