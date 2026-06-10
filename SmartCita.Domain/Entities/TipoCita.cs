using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCita.Domain.Entities
{
    public class TipoCita
    {
        private TipoCita() { }

        public TipoCita(string nombre, string descripcion, decimal precioBase, int duracionMinutos)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre es obligatorio");
            if (precioBase < 0) throw new ArgumentException("El precio no puede ser negativo");
            if (duracionMinutos <= 0) throw new ArgumentException("La duración debe ser mayor a 0");

            Nombre = nombre;
            Descripcion = descripcion;
            PrecioBase = precioBase;
            DuracionMinutos = duracionMinutos;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Nombre { get; private set; } = string.Empty;
        public string? Descripcion { get; private set; } = string.Empty;
        public decimal PrecioBase { get; private set; }
        public int? DuracionMinutos { get; private set; }
        public bool EstaActivo { get; set; } = true;
    }
}
