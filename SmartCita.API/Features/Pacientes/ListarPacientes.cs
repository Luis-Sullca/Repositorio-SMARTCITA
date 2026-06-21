using Microsoft.EntityFrameworkCore;
using SmartCita.API.Common.Endpoints;
using SmartCita.Infrastructure.Persistence;

namespace SmartCita.API.Features.Pacientes
{
    public class ListarPacientes : IEndpoint
    {
        public record Response(
            Guid Id, 
            string Nombres, 
            string Apellidos, 
            string Dni, 
            string? Telefono,
            string? Email,
            DateOnly FechaNacimiento,
            bool EstaActivo
            );

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/pacientes", async (AppDbContext context, CancellationToken cancellationToken) =>
            {
                var pacientes = await context.Pacientes
                    .AsNoTracking()
                    .OrderByDescending(p => p.FechaNacimiento)
                    .Select(p => new Response(
                        p.Id, 
                        p.Nombres, 
                        p.Apellidos, 
                        p.Dni, 
                        p.Telefono, 
                        p.Email, 
                        p.FechaNacimiento, 
                        p.EstaActivo))
                    .ToListAsync(cancellationToken);
                return Results.Ok(pacientes);
            })
            .WithName("ListarPacientes")
            .WithTags("Pacientes");
        }
    }
}
    