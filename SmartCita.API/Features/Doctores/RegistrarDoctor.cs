using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCita.API.Common.Endpoints;
using SmartCita.Domain.Entities;
using SmartCita.Domain.Enums;
using SmartCita.Infrastructure.Persistence;

namespace SmartCita.API.Features.Doctores
{
    public class RegistrarDoctor : IEndpoint
    {
        public record Command
            (
            string Nombre,
            string Apellido,
            string NumeroColegiatura,
            string UniversidadEgresado,
            EspecialidadMedica Especialidad
            ) : IRequest<Guid>;

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
                RuleFor(x => x.Apellido).NotEmpty().MaximumLength(100);

                // Validación del Colegio Médico (Generalmente son 5 a 6 dígitos en Perú)
                RuleFor(x => x.NumeroColegiatura)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("El número de colegiatura (CMP) es obligatorio.")
                    .Matches(@"^\d{5,6}$").WithMessage("El CMP debe tener entre 5 y 6 dígitos numéricos.");

                RuleFor(x => x.Especialidad).IsInEnum().WithMessage("La especialidad seleccionada no es válida.");
            }
        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly AppDbContext _context;

            public Handler(AppDbContext context) => _context = context;

            public async Task<Guid> Handle(Command command, CancellationToken cancellationToken)
            {
                bool cpmExiste = await _context.Doctores
                    .AnyAsync(d => d.NumeroColegiatura == command.NumeroColegiatura, cancellationToken);

                if (cpmExiste)
                {
                    throw new InvalidOperationException("Ya existe un doctor registrado con este numero de colegiatura (CPM)");
                }

                var nuevoDoctor = new Doctor
                    (
                    command.Nombre,
                    command.Apellido,
                    command.NumeroColegiatura,
                    command.UniversidadEgresado,
                    command.Especialidad
                    );

                _context.Doctores.Add(nuevoDoctor);
                await _context.SaveChangesAsync(cancellationToken);

                return nuevoDoctor.Id;
            }
        }

        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/doctores", async (Command command, IMediator mediator) =>
            {
                var doctorId = await mediator.Send(command);
                return Results.Created($"/api/doctores/{doctorId}", new { Id = doctorId });
            }).WithName("RegistrarDoctor").WithTags("Doctores");
        }
    }
}
