using SmartCita.API.Common.Endpoints;
using SmartCita.Domain.Enums;

namespace SmartCita.API.Features.Doctores
{
    public class ListarEspecialidades : IEndpoint
    {

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/doctores/especialidades", () =>
            {
                var especialidades = Enum.GetValues<EspecialidadMedica>()
                    .Select(e => new
                    {
                        Valor = e.ToString(),
                        Nombre = e.ToString()
                    });
                    
                return Results.Ok(especialidades);
            }).WithName("ListarEspecialidades").WithTags("Doctores");
        }
    }
}
