namespace Farewell.Abstractions.CQRS;

public record OperationResult<T>(int Code, string Message, T? Result);

public record OperationResult(int Code, string Message);