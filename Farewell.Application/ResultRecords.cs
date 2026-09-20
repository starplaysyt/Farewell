namespace Farewell.Application;

public record OperationResult<T>(int Code, string Message, T? Result);

public record OperationResult(int Code, string Message);

public record QueryResult<T>(int Code, string Message, T? Result);

public record QueryResult(int Code, string Message);

public record ServiceResult<T>(int Code, string Message, T? Result);

public record ServiceResult(int Code, string Message);