using caMUNICIPIOSAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Infraestructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<CierreCaja> CierreCaja { get; set; }
        public DbSet<ColaNotificaciones> ColaNotificaciones { get; set; }
        public DbSet<ComprobantesMercadoPago> ComprobantesMercadoPago { get; set; }
        public DbSet<Contribuyente> Contribuyentes { get; set; }
        public DbSet<ContribuyentesImpuestosVariables> ContribuyentesImpuestosVariables { get; set; }
        public DbSet<CuotasPlan> CuotasPlan { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<EstadoTributo> EstadoTributos { get; set; }
        public DbSet<Factura> Factura { get; set; }
        public DbSet<InicioCaja> InicioCaja { get; set; }
        public DbSet<Inmueble> Inmuebles { get; set; }
        public DbSet<InmuebleObra> InmueblesObras { get; set; }
        public DbSet<IntegracionExterna> IntegracionesExternas { get; set; }
        public DbSet<InteresMora> InteresesMora { get; set; }
        public DbSet<Localidad> Localidades { get; set; }
        public DbSet<MedioPago> MediosPago { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Noticias> Noticias { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<ObraPublica> ObrasPublicas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<PlanPago> PlanesPago { get; set; }
        public DbSet<Periodicidad> Periodicidades { get; set; }
        public DbSet<PlantillaNotificacion> PlantillasNotificaciones { get; set; }
        public DbSet<Recibo> Recibo { get; set; }
        public DbSet<RolPermiso> RolesPermisos { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<TipoDocumento> TiposDocumento { get; set; }
        public DbSet<TipoImpuesto> TiposImpuesto { get; set; }
        public DbSet<TipoInmueble> TiposInmueble { get; set; }
        public DbSet<TitularidadInmueble> TitularidadesInmueble { get; set; }
        public DbSet<Tributo> Tributos { get; set; }
        public DbSet<TributoInmobiliario> TributosInmobiliarios { get; set; }
        public DbSet<UsuarioRol> UsuariosRoles { get; set; }
        public DbSet<ValorTipoImpuesto> ValoresTipoImpuesto { get; set; }
        public DbSet<ValuacionInmueble> ValuacionesInmueble { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuarios>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Auditoria>(entity =>
            {
                entity.ToTable("Auditoria");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<CierreCaja>(entity =>
            {
                entity.ToTable("CierreCaja");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ComprobantesMercadoPago>(entity =>
            {
                entity.ToTable("ComprobantesMercadoPago");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ColaNotificaciones>(entity =>
            {
                entity.ToTable("ColaNotificaciones");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IdPlantilla).HasColumnName("id_plantilla");
                entity.Property(e => e.IdContribuyente).HasColumnName("id_contribuyente");
                entity.Property(e => e.CelularDestino).HasColumnName("celular_destino");
                entity.Property(e => e.Canal).HasColumnName("canal");
                entity.Property(e => e.FechaProgramada).HasColumnName("fecha_programada");
                entity.Property(e => e.EstadoEnvio).HasColumnName("estado_envio");
                entity.Property(e => e.Intento).HasColumnName("intento");
            });

            modelBuilder.Entity<Contribuyente>(entity =>
            {
                entity.ToTable("Contribuyentes");
                entity.HasKey(e => e.Id);
                entity.Property(c => c.FechaAlta).HasDefaultValueSql("GETDATE()");

            });

            modelBuilder.Entity<ContribuyentesImpuestosVariables>(entity =>
            {
                entity.ToTable("ContribuyentesImpuestosVariables");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<CuotasPlan>(entity =>
            {
                entity.ToTable("CuotasPlan");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Estado>(entity =>
            {
                entity.ToTable("Estados");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<EstadoTributo>(entity =>
            {
                entity.ToTable("EstadoTributo");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Factura>(entity =>
            {
                entity.ToTable("Facturas");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<InicioCaja>(entity =>
            {
                entity.ToTable("InicioCaja");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaInicioCaja).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<Inmueble>(entity =>
            {
                entity.ToTable("Inmuebles");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<InmuebleObra>(entity =>
            {
                entity.ToTable("InmueblesObras");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<IntegracionExterna>(entity =>
            {
                entity.ToTable("Integraciones_Externas");
                entity.HasKey(e => e.IdIntegracion);

                entity.Property(e => e.IdIntegracion)
                    .HasColumnName("id_integracion");

                entity.Property(e => e.NombreSistema)
                    .HasColumnName("nombre_sistema");

                entity.Property(e => e.Descripcion)
                    .HasColumnName("descripcion");

                entity.Property(e => e.UrlApi)
                    .HasColumnName("url_api");

                entity.Property(e => e.TokenAcceso)
                    .HasColumnName("token_acceso");

                entity.Property(e => e.FechaRegistro)
                    .HasColumnName("fecha_registro");
            });

            modelBuilder.Entity<InteresMora>(entity =>
            {
                entity.ToTable("InteresesMora");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Localidad>(entity =>
            {
                entity.ToTable("Localidades");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<MedioPago>(entity =>
            {
                entity.ToTable("MediosPago");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Municipio>(entity =>
            {
                entity.ToTable("Municipios");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Noticias>(entity =>
            {
                entity.ToTable("Noticias");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Notificacion>(entity =>
            {
                entity.ToTable("Notificacion");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ObraPublica>(entity =>
            {
                entity.ToTable("Obras_Publicas");
                entity.HasKey(e => e.IdObra);
                entity.Property(e => e.IdObra)
                    .HasColumnName("id_obra");
                entity.Property(e => e.IdMunicipio)
                    .HasColumnName("id_municipio");
                entity.Property(e => e.FechaInicio)
                    .HasColumnName("fecha_inicio");
                entity.Property(e => e.FechaFin)
                    .HasColumnName("fecha_fin");
                entity.Property(e => e.CostoTotal)
                    .HasColumnName("costo_total");
                entity.Property(e => e.IdEstado)
                    .HasColumnName("id_estado");
            });

            modelBuilder.Entity<Pago>(entity =>
            {
                entity.ToTable("Pagos");
                entity.HasKey(e => e.IdPago);
                entity.Property(e => e.IdPago)
                    .HasColumnName("id_pago");
                entity.Property(e => e.IdContribuyente)
                    .HasColumnName("id_contribuyente");
                entity.Property(e => e.IdTributo)
                    .HasColumnName("id_tributo");
                entity.Property(e => e.FechaPago)
                    .HasColumnName("fecha_pago");
                entity.Property(e => e.MontoPagado)
                    .HasColumnName("monto_pagado");
                entity.Property(e => e.IdMedioPago)
                    .HasColumnName("id_medio_pago");
                entity.Property(e => e.IdCierre)
                    .HasColumnName("IdCierre");
            });

            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.ToTable("Permisos");
                entity.HasKey(e => e.IdPermiso);
                entity.Property(e => e.IdPermiso)
                    .HasColumnName("id_permiso");
                entity.Property(e => e.NombrePermiso)
                    .HasColumnName("nombre_permiso");
            });

            modelBuilder.Entity<PlanPago>(entity =>
            {
                entity.ToTable("PlanPago");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Periodicidad>(entity =>
            {
                entity.ToTable("Periodicidad");
                entity.HasKey(e => e.IdPeriodicidad);
                entity.Property(e => e.IdPeriodicidad)
                    .HasColumnName("id_periodicidad");
            });

            modelBuilder.Entity<PlantillaNotificacion>(entity =>
            {
                entity.ToTable("PlantillaNotificacion");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Recibo>(entity =>
            {
                entity.ToTable("Recibos");
                entity.HasKey(e => e.Id);
            });
            

            modelBuilder.Entity<RolPermiso>(entity =>
            {
                entity.ToTable("Rol_Permiso");
                entity.HasKey(e => new { e.IdRol, e.IdPermiso });
                entity.Property(e => e.IdRol)
                    .HasColumnName("id_rol");
                entity.Property(e => e.IdPermiso)
                    .HasColumnName("id_permiso");
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.IdRol);
                entity.Property(e => e.IdRol)
                    .HasColumnName("id_rol");
                entity.Property(e => e.NombreRol)
                    .HasColumnName("nombre_rol");
            });

            modelBuilder.Entity<TipoDocumento>(entity =>
            {
                entity.ToTable("TipoDocumento");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<TipoImpuesto>(entity =>
            {
                entity.ToTable("TipoImpuesto");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<TipoInmueble>(entity =>
            {
                entity.ToTable("TipoInmueble");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<TitularidadInmueble>(entity =>
            {
                entity.ToTable("Titularidad_Inmueble");
                entity.HasKey(e => e.IdTitularidad);
                entity.Property(e => e.IdTitularidad)
                    .HasColumnName("id_titularidad");
                entity.Property(e => e.IdInmueble)
                    .HasColumnName("id_inmueble");
                entity.Property(e => e.IdContribuyente)
                    .HasColumnName("id_contribuyente");
                entity.Property(e => e.FechaInicio)
                    .HasColumnName("fecha_inicio");
                entity.Property(e => e.FechaFin)
                    .HasColumnName("fecha_fin");
            });

            modelBuilder.Entity<Tributo>(entity =>
            {
                entity.ToTable("Tributos");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<TributoInmobiliario>(entity =>
            {
                entity.ToTable("Tributos_Inmobiliarios");
                entity.HasKey(e => e.IdTributo);
                entity.Property(e => e.IdTributo)
                    .HasColumnName("id_tributo");
                entity.Property(e => e.IdMunicipio)
                    .HasColumnName("id_municipio");
                entity.Property(e => e.IdInmueble)
                    .HasColumnName("id_inmueble");
                entity.Property(e => e.IdTipoImpuesto)
                    .HasColumnName("id_tipo_impuesto");
                entity.Property(e => e.FechaEmision)
                    .HasColumnName("fecha_emision");
                entity.Property(e => e.FechaVencimiento)
                    .HasColumnName("fecha_vencimiento");
                entity.Property(e => e.IdEstado)
                    .HasColumnName("id_estado");
            });

            modelBuilder.Entity<UsuarioRol>(entity =>
            {
                entity.ToTable("Usuario_Rol");
                entity.HasKey(e => new { e.IdUsuario, e.IdRol });
                entity.Property(e => e.IdUsuario)
                    .HasColumnName("id_usuario");

                entity.Property(e => e.IdRol)
                    .HasColumnName("id_rol");
            });

            modelBuilder.Entity<ValorTipoImpuesto>(entity =>
            {
                entity.ToTable("ValorTipoImpuesto");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ValuacionInmueble>(entity =>
            {
                entity.ToTable("Valuaciones_Inmueble");
                entity.HasKey(e => e.IdValuacion);
                entity.Property(e => e.IdValuacion)
                    .HasColumnName("id_valuacion");
                entity.Property(e => e.IdInmueble)
                    .HasColumnName("id_inmueble");
                entity.Property(e => e.FechaValuacion)
                    .HasColumnName("fecha_valuacion");

            });

        }
    }
}
