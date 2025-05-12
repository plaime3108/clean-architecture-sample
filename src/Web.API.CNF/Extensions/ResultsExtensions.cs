using Application.Responses;

namespace Web.API.CNF.Extensions
{
    public static class ResultsExtensions
    {
        public static IResult Failure(this IResultExtensions results, ErrorResponse result)
        {
            return Results.Json(new Models.Responses.ErrorResponse { Error = result.Message, Details = result.ErrorDetails }, statusCode: (int)result.StatusCode);
        }
    }
}
