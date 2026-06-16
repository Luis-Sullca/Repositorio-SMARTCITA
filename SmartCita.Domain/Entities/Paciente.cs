using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCita.Domain.Entities
{
    public class Paciente
    {
        private Paciente() { }
        public Paciente(string nombres, string apellidos, string dni, DateOnly fechaNacimiento)
        {
            if (string.IsNullOrWhiteSpace(nombres)) throw new ArgumentException("El nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(apellidos)) throw new ArgumentException("El apellido es obligatorio");
            if (string.IsNullOrWhiteSpace(dni)) throw new ArgumentException("El DNI es obligatorio");
            if (fechaNacimiento > DateOnly.FromDateTime(DateTime.UtcNow)) throw new ArgumentException("La fecha de nacimiento no puede ser en el futuro");

            Nombres = nombres;
            Apellidos = apellidos;
            Dni = dni;
            FechaNacimiento = fechaNacimiento;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Nombres { get; private set; } = string.Empty;
        public string Apellidos { get; private set; } = string.Empty;
        public string Dni { get; private set; } = string.Empty;
        public DateOnly FechaNacimiento { get; private set; }
        public DateTime FechaRegistro { get; private set; } = DateTime.UtcNow;

        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Alergias { get; set; }
        public string? MedicamentosActuales { get; set; }
        public string? HistorialMedico { get; set; }

        public bool EstaActivo { get; private set; } = true;

        // Relación de navegación: Un paciente tiene muchas citas
        // Se puede usar HashSet en vez de List para mejorar el rendimiento en ciertas operaciones, pero List es más común y fácil de usar
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
