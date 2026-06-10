using SmartCita.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCita.Domain.Entities
{
    public class Horario
    {
        private Horario() { }

        public Horario(Guid doctorId, DateOnly fecha, TimeOnly horaInicio, TimeOnly horaFin)
        {
            if (doctorId == Guid.Empty) throw new ArgumentException("El doctor es obligatorio");
            if (horaInicio >= horaFin) throw new ArgumentException("La hora de inicio debe ser anterior a la hora de fin");

            DoctorId = doctorId;
            Fecha = fecha;
            HoraInicio = horaInicio;
            HoraFin = horaFin;
            Estado = EstadoHorario.Disponible;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid DoctorId { get; private set; }

        // Uso de tipos modernos de .NET 8 (Mucho más eficientes que DateTime)
        public DateOnly Fecha { get; private set; }
        public TimeOnly HoraInicio { get; private set; }
        public TimeOnly HoraFin { get; private set; }

        public EstadoHorario Estado { get; private set; }

        // Propiedades de Navegación
        public Doctor? Doctor { get; set; }
        public Cita? Cita { get; set; }

        // Método de negocio para marcar el horario como ocupado (reservado)
        public void MarcarComoOcupado()
        {
            if (Estado == EstadoHorario.Reservado)
                throw new InvalidOperationException("Error crítico: Este bloque de tiempo ya fue reservado.");

            Estado = EstadoHorario.Reservado;
        }

        public void LiberarHorario()
        {
            Estado = EstadoHorario.Disponible;
        }

        public void BloquearPorAdministracion() { Estado = EstadoHorario.NoDisponible; }
    }
}
