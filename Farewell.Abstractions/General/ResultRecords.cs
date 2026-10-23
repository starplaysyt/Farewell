using Farewell.Abstractions.Validation;

namespace Farewell.Abstractions.General;

public record OperationResult(ResultCode Code, string Message)
    : OperationResult<object?>(Code, Message, null)
{
    public new static readonly OperationResult Ok = new(ResultCode.Ok, "Success");

    public OperationResult(ResultCode Code) : this(Code, Code.ToString())
    {
    }
}

public record OperationResult<TResult>(ResultCode Code, string Message, TResult Result)
{
    public static OperationResult<TResult> Ok(TResult result) => new(ResultCode.Ok, result);
    
    public OperationResult(ResultCode Code, TResult result = default)
        : this(Code, "", result)
    {
    }
};