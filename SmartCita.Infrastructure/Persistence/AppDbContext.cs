using Microsoft.EntityFrameworkCore;
using SmartCita.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCita.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Doctor> Doctores { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<TipoCita> TiposCita { get; set; }
        public DbSet<Horario> Horarios { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Paciente
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(p => p.Id);

                // Nombres y Apellidos obligatorios
                entity.Property(p => p.Nombres).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Apellidos).IsRequired().HasMaxLength(100);

                // DNI es obligatorio y único para evitar duplicados
                entity.Property(p => p.Dni).IsRequired().HasMaxLength(8);
                entity.HasIndex(p => p.Dni).IsUnique();

                entity.Property(p => p.Telefono).HasMaxLength(20);
                entity.Property(p => p.Email).HasMaxLength(100);

                entity.Property(p => p.Alergias).HasMaxLength(300);
                entity.Property(p => p.MedicamentosActuales).HasMaxLength(500);
                entity.Property(p => p.HistorialMedico).HasMaxLength(1000);
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(d => d.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Apellido).IsRequired().HasMaxLength(100);

                entity.Property(d => d.NumeroColegiatura).IsRequired().HasMaxLength(50);
                entity.HasIndex(d => d.NumeroColegiatura).IsUnique();

                entity.Property(d => d.UniversidadEgresado).HasMaxLength(150);

                entity.Property(d => d.Especialidad).HasConversion<string>().HasMaxLength(50);
            });

            modelBuilder.Entity<TipoCita>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Descripcion).HasMaxLength(250);
                entity.Property(t => t.PrecioBase).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Horario>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Estado).HasConversion<string>().HasMaxLength(20);

                entity.HasOne(h => h.Doctor)
                      .WithMany()
                      .HasForeignKey(h => h.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Cita>(entity =>
            {
                entity.HasKey(c => c.Id);

                // Relaciones: Evitamos borrados en cascada para proteger la integridad del historial
                entity.HasOne(c => c.Paciente)
                      .WithMany(p => p.Citas)
                      .HasForeignKey(c => c.PacienteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Doctor)
                      .WithMany(d => d.Citas)
                      .HasForeignKey(c => c.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.TipoCita)
                      .WithMany()
                      .HasForeignKey(c => c.TipoCitaId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación 1 a 1 con Horario
                entity.HasOne(c => c.Horario)
                      .WithOne(h => h.Cita)
                      .HasForeignKey<Cita>(c => c.HorarioId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.MotivoConsulta).IsRequired().HasMaxLength(250);
                entity.Property(c => c.Estado).IsRequired().HasConversion<string>().HasMaxLength(50);
            });

        }
    }
}
