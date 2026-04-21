using System.Runtime.CompilerServices;

namespace Shared.Models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error Error { get; }

    private Result(bool isSuccess, T? value, Error error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, Error.None);
    public static Result<T> Failure(Error error) => new(false, default, error);
}
public sealed record Error(string Code, string Description, string? StackTrace =null)
{
    public static readonly Error None= new(string.Empty,string.Empty);

    public static Error Unexpected(string code,string description)=> new (code,description, Environment.StackTrace);

}