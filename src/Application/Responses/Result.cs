using Application.Common.Enums;

namespace Application.Responses
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public ErrorResponse? Error { get; }

        protected Result(bool isSuccess, T? value, ErrorResponse? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(true, value, null);
        public static Result<T> Failure(string message, HttpStatusCode statusCode, string? details = null)
            => new Result<T>(false, default, new ErrorResponse(message, statusCode, details));
    }
}
