using FluentValidation;
using MediatR;
using SmartCita.Domain.Entities;
using SmartCita.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SmartCita.API.Features.Pacientes
{
    public class RegistrarPaciente
    {
        /// <summary>
        /// Comando para registrar un nuevo paciente en el sistema. 
        /// Contiene toda la información necesaria para crear un nuevo registro de paciente, 
        /// incluyendo datos personales, información de contacto y detalles médicos relevantes.
        /// </summary>
        /// <param name="Nombres">Nombres del paciente.</param>
        /// <param name="Apellidos">Apellidos del paciente.</param>
        /// <param name="Dni">Documento Nacional de Identidad del paciente.</param>
        /// <param name="FechaNacimiento">Fecha de nacimiento del paciente.</param>
        /// <param name="Telefono"></param>
        /// <param name="Email"></param>
        /// <param name="Alergias"></param>
        /// <param name="MedicamentosActuales"></param>
        /// <param name="HistorialMedico"></param>

        public record Command
            (
            string Nombres,
            string Apellidos,
            string Dni,
            DateOnly FechaNacimiento,
            string? Telefono,
            string? Email,
            string? Alergias,
            string? MedicamentosActuales,
            string? HistorialMedico
            ) : IRequest<Guid>; // El comando devuelve el ID del paciente registrado


        /// <summary>
        /// Validador para el comando de registrar paciente. Define las reglas de validación para cada propiedad del comando,
        /// </summary>
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Nombres).NotEmpty().MaximumLength(100);
                RuleFor(x => x.Apellidos).NotEmpty().MaximumLength(100);

                // Reglas exactas para el DNI: es obligatorio, no puede estar vacío y debe tener un máximo de 8 caracteres
                RuleFor(x => x.Dni)
                    .NotEmpty()
                    .Length(8)
                    .WithMessage("El DNI debe contener exactamente 8 digitos")
                    .Matches("^[0-9]*$")
                    .WithMessage("El DNI debe contener solo numeros");

                RuleFor(x => x.FechaNacimiento)
                    .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                    .WithMessage("La fecha de nacimiento no puede ser en el futuro");

                RuleFor(x => x.Email)
                    .EmailAddress()
                    .When(x => !string.IsNullOrWhiteSpace(x.Email));

                RuleFor(x => x.Telefono).MaximumLength(20);

                RuleFor(x => x.Alergias).MaximumLength(300);

                RuleFor(x => x.MedicamentosActuales).MaximumLength(500);

                RuleFor(x => x.HistorialMedico).MaximumLength(1000);
            }
        }


        /// <summary>
        /// Manejador para el comando de registrar paciente. 
        /// Contiene la lógica para procesar el comando, 
        /// incluyendo la validación de datos 
        /// y la interacción con la base de datos para crear un nuevo registro de paciente.
        /// </summary>
        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly AppDbContext _context;
            public Handler(AppDbContext context)
            {
                _context = context;
            }

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                bool dniExistente = await _context.Pacientes.AnyAsync(p => p.Dni == request.Dni, cancellationToken);
                if (dniExistente)
                {
                    throw new InvalidOperationException("Ya existe un paciente registrado con el mismo DNI.");
                }

                // Lógica para registrar el paciente utilizando el contexto de la base de datos
                var nuevoPaciente = new Paciente
                (
                    request.Nombres,
                    request.Apellidos,
                    request.Dni,
                    request.FechaNacimiento
                );

                nuevoPaciente.Telefono = request.Telefono;
                nuevoPaciente.Email = request.Email;
                nuevoPaciente.Alergias = request.Alergias;
                nuevoPaciente.MedicamentosActuales = request.MedicamentosActuales;
                nuevoPaciente.HistorialMedico = request.HistorialMedico;

                _context.Pacientes.Add(nuevoPaciente);
                await _context.SaveChangesAsync(cancellationToken);

                return nuevoPaciente.Id;
            }
        }

        /// <summary>
        /// Método para mapear el endpoint de registrar paciente en la aplicación.
        /// </summary>
        /// <param name="app">El constructor de rutas de la aplicación.</param>
        public static void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/pacientes", async (Command command, IMediator mediator) =>
            {
                try
                {
                    var pacienteId = await mediator.Send(command);
                    return Results.Created($"/api/pacientes/{pacienteId}", new { Id = pacienteId });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { Error = ex.Message });
                }
            })
            .WithTags("Pacientes")
            .WithName("RegistrarPaciente");
        }
    }
}
