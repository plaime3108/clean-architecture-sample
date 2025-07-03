namespace Application.Interfaces.External
{
    public interface IExternalApiClient
    {
        Task<TResponse> SendAsync<TResponse>();
        Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, string path);
    }
}
