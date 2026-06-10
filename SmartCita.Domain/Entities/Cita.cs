using SmartCita.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCita.Domain.Entities
{
    public class Cita
    {
        private Cita() { }

        // 2. Constructor de Negocio (El único que usará tu equipo)
        public Cita(Guid pacienteId, Guid doctorId, Guid horarioId, Guid tipoCitaId)
        {
            if (pacienteId == Guid.Empty) throw new ArgumentException("El paciente es obligatorio");
            if (doctorId == Guid.Empty) throw new ArgumentException("El doctor es obligatorio");
            if (horarioId == Guid.Empty) throw new ArgumentException("El horario es obligatorio");
            if (tipoCitaId == Guid.Empty) throw new ArgumentException("El tipo de cita es obligatorio");

            PacienteId = pacienteId;
            DoctorId = doctorId;
            HorarioId = horarioId; // Reemplaza a las fechas sueltas
            TipoCitaId = tipoCitaId;
            Estado = EstadoCita.Programada;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid PacienteId { get; private set; }
        public Guid DoctorId { get; private set; }
        public Guid HorarioId { get; private set; }

        public Guid TipoCitaId { get; private set; }

        // El motivo sí puede ser editado públicamente después de crear la cita
        public string? MotivoConsulta { get; set; } = string.Empty;
        public EstadoCita Estado { get; private set; }

        // Propiedades de navegación
        public Paciente? Paciente { get; set; }
        public Doctor? Doctor { get; set; }
        public Horario? Horario { get; set; }
        public TipoCita? TipoCita { get; set; }
    }
}
