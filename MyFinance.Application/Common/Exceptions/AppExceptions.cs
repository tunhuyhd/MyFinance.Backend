namespace MyFinance.Application.Common.Exceptions;

public class NotFoundException(string name, object key)
    : Exception($"Entity \"{name}\" ({key}) was not found.");

public class ForbiddenAccessException()
    : Exception("Access is forbidden.");

public class ConflictException(string message) : Exception(message);
