namespace Farewell.Application;

public record CommandResult<T>(int Code, string Message, T? Result);

public record CommandResult(int Code, string Message);

public record QueryResult<T>(int Code, string Message, T? Result);

public record QueryResult(int Code, string Message);

public record ServiceResult<T>(int Code, string Message, T? Result);

public record ServiceResult(int Code, string Message);