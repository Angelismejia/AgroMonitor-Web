using Microsoft.EntityFrameworkCore;
using AgroMonitor.Models;
using System;

namespace AgroMonitor.Data
{
    public class AgroMonitorDbContext : DbContext
    {
        public AgroMonitorDbContext(DbContextOptions<AgroMonitorDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Finca> Fincas { get; set; }
        public DbSet<Cultivo> Cultivos { get; set; }
        public DbSet<Sensor> Sensores { get; set; }
        public DbSet<LecturaSensor> LecturasSensor { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<RecomendacionML> RecomendacionesML { get; set; }
        public DbSet<Reporte> Reportes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(e => e.Correo).IsUnique();
            });

            modelBuilder.Entity<Finca>(entity =>
            {
                entity.HasOne(f => f.Usuario)
                    .WithMany(u => u.Fincas)
                    .HasForeignKey(f => f.IdUsuario)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Cultivo>(entity =>
            {
                entity.HasOne(c => c.Finca)
                    .WithMany(f => f.Cultivos)
                    .HasForeignKey(c => c.IdFinca)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Sensor>(entity =>
            {
                entity.HasOne(s => s.Finca)
                    .WithMany(f => f.Sensores)
                    .HasForeignKey(s => s.IdFinca)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<LecturaSensor>(entity =>
            {
                entity.HasOne(l => l.Sensor)
                    .WithMany(s => s.Lecturas)
                    .HasForeignKey(l => l.IdSensor)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Alerta>(entity =>
            {
                entity.HasOne(a => a.Cultivo)
                    .WithMany(c => c.Alertas)
                    .HasForeignKey(a => a.IdCultivo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Finca)
                    .WithMany(f => f.Alertas)
                    .HasForeignKey(a => a.IdFinca)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RecomendacionML>(entity =>
            {
                entity.HasOne(r => r.Cultivo)
                    .WithMany(c => c.Recomendaciones)
                    .HasForeignKey(r => r.IdCultivo)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Reporte>(entity =>
            {
                entity.HasOne(r => r.Usuario)
                    .WithMany(u => u.Reportes)
                    .HasForeignKey(r => r.IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Finca)
                    .WithMany(f => f.Reportes)
                    .HasForeignKey(r => r.IdFinca)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}