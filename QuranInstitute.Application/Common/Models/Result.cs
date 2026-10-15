namespace QuranInstitute.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }

    protected Result(bool success, string? error)
    {
        IsSuccess = success;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}

public class Result<T> : Result
{
    public T? Value { get; init; }

    private Result(bool success, T? value, string? error) : base(success, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static new Result<T> Failure(string error) => new(false, default, error);
}