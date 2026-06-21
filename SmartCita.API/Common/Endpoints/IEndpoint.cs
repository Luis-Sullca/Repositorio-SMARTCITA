
namespace SmartCita.API.Common.Endpoints
{
    public interface IEndpoint
    {
        static abstract void MapEndpoint(IEndpointRouteBuilder app);
    }
}
