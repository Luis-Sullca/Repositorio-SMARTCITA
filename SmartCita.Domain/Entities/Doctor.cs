using SmartCita.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCita.Domain.Entities
{
    public class Doctor
    {
        private Doctor() { }

        public Doctor(string nombre, string apellido, string numeroColegiatura, string universidadEgresado, EspecialidadMedica especialidad)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(apellido)) throw new ArgumentException("El apellido es obligatorio"); 
            if (string.IsNullOrWhiteSpace(numeroColegiatura)) throw new ArgumentException("El número de colegiatura es obligatorio");
            if (!Enum.IsDefined(typeof(EspecialidadMedica), especialidad))
                throw new ArgumentException("La especialidad médica seleccionada no es válida");

            Nombre = nombre;
            Apellido = apellido;
            NumeroColegiatura = numeroColegiatura;
            UniversidadEgresado = universidadEgresado;
            Especialidad = especialidad;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Nombre { get; private set; } = string.Empty;
        public string Apellido { get; private set; } = string.Empty;
        public EspecialidadMedica Especialidad { get; private set; }
        public string NumeroColegiatura { get; private set; } = string.Empty;
        public string UniversidadEgresado { get; private set; } = string.Empty;
        public bool EstaActivo { get; set; } = true;

        // Relación de navegación: Un doctor tiene muchas citas
        // Se puede usar HashSet en vez de List para mejorar el rendimiento en ciertas operaciones, pero List es más común y fácil de usar
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
