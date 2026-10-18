namespace Farewell.Abstractions.General;

public record OperationResult(ResultCode Code, string Message)
    : OperationResult<object?>(Code, Message, null)
{
    public OperationResult(ResultCode Code) : this(Code, Code.ToString()) { }
}

public record OperationResult<T>(ResultCode Code, string Message, T Result)
{
};

