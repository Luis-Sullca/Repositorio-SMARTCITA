using Microsoft.EntityFrameworkCore;
using SmartCita.API.Common.Endpoints;
using SmartCita.Infrastructure.Persistence;

namespace SmartCita.API.Features.Doctores
{
    public class ListarDoctores : IEndpoint
    {
        public record Response
            (
            Guid Id,
            string Nombre,
            string Apellido,
            string NumeroColegiatura,
            string UniversidadEgresado,
            string Especialidad,
            bool EstaActivo
            );

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/doctores", async (AppDbContext context, CancellationToken cancellationToken) =>
            {
                var doctores = await context.Doctores
                    .AsNoTracking()
                    .OrderBy(d => d.Apellido)
                    .ThenBy(d => d.Nombre)
                    .Select(d => new Response(
                        d.Id,
                        d.Nombre,
                        d.Apellido,
                        d.NumeroColegiatura,
                        d.UniversidadEgresado,
                        d.Especialidad.ToString(),
                        d.EstaActivo))
                    .ToListAsync(cancellationToken);
                return Results.Ok(doctores);
            }).WithTags("Doctores").WithName("ListarDoctores");
        }
    }
}
