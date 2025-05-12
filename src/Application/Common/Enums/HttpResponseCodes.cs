namespace Application.Common.Enums
{
    public enum HttpStatusCode
    {
        OK = 200,
        Created = 201,
        NoContent = 204,
        MovedPermanently = 301,
        Found = 302,
        TemporaryRedirect = 307,
        PermanentRedirect = 308,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        RequestTimeOut = 408,
        Conflict = 409,
        Gone = 410,
        PreconditionFailed = 412,
        UnsupportedMediaType = 415,
        UnprocessableEntity = 422,
        TooEarly = 425,
        TooManyRequest = 429,
        InternalServerError = 500,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeOut = 504,
        InsufficientStorage = 507
    }
}
