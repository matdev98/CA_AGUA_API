using AutoMapper;
using caMUNICIPIOSAPI.Application.DTOs;
using caMUNICIPIOSAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace caMUNICIPIOSAPI.Application.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // USERS - DTOs
            CreateMap<Usuarios, UserDTO>();
            CreateMap<UserCreateDTO, Usuarios>();
            CreateMap<UserUpdateDTO, Usuarios>();
            CreateMap<UserUpdateDTO, Usuarios>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // MUNICIPIOS - DTOs
            CreateMap<Municipio, MunicipioDTO>().ReverseMap();

            // CONTRIBUYENTES - DTOs
            CreateMap<Contribuyente, ContribuyenteDTO>().ReverseMap()
                .ForMember(dest => dest.FechaAlta, opt => opt.Ignore());

            //AUDITORIAS - DTOs
            CreateMap<Auditoria, AuditoriaDTO>().ReverseMap();

            //CUOTASPLAN - DTOs
            CreateMap<CuotasPlan, CuotasPlanDTO>().ReverseMap();

            //ESTADO - DTOs
            CreateMap<Estado, EstadoDTO>().ReverseMap();

            //ESTADOTRIBUTO - DTOs
            CreateMap<EstadoTributo, EstadoTributoDTO>().ReverseMap();

            //INMUEBLE - DTOs
            CreateMap<Inmueble, InmuebleDTO>().ReverseMap();

            //INMUEBLEOBRA - DTOs
            CreateMap<InmuebleObra, InmuebleObraDTO>().ReverseMap();

            //INTEGRACIONEXTERNA - DTOSs
            CreateMap<IntegracionExterna, IntegracionExternaDTO>().ReverseMap();

            //COLANOTIFICACIONES - DTOs
            CreateMap<ColaNotificaciones, ColaNotificacionesDTO>().ReverseMap();

            // Periodicidades - DTOs
            CreateMap<Periodicidad, PeriodicidadDTO>().ReverseMap();

            // PlantillasNotificaciones - DTOs
            CreateMap<PlantillaNotificacion, PlantillaNotificacionDTO>().ReverseMap();

            // RolesPermisos - DTOs
            CreateMap<RolPermiso, RolPermiso>().ReverseMap();

            // Roles - DTOs
            CreateMap<Rol, RolDTO>().ReverseMap();

            // TiposDocumento - DTOs
            CreateMap<TipoDocumento, TipoDocumentoDTO>().ReverseMap();

            // TiposImpuesto - DTOs
            CreateMap<TipoImpuesto, TipoImpuestoDTO>().ReverseMap();

            // TiposInmueble - DTOs
            CreateMap<TipoInmueble, TipoInmuebleDTO>().ReverseMap();

            // TitularidadesInmueble - DTOs
            CreateMap<TitularidadInmueble, TitularidadInmuebleDTO>().ReverseMap();

            // Tributos - DTOs
            CreateMap<Tributo, TributoDTO>().ReverseMap();

            CreateMap<TributoContribuyenteDTO, TributoDTO>().ReverseMap();

            CreateMap<Tributo, TributoContribuyenteDTO>().ReverseMap();

            // TributosInmobiliarios - DTOs
            CreateMap<TributoInmobiliario, TributoInmobiliarioDTO>().ReverseMap();

            // UsuariosRoles - DTOs
            CreateMap<UsuarioRol, UsuarioRolDTO>().ReverseMap();

            // ValoresTipoImpuesto - DTOs
            CreateMap<ValorTipoImpuesto, ValorTipoImpuestoDTO>().ReverseMap();

            // ValuacionesInmueble - DTOs
            CreateMap<ValuacionInmueble, ValuacionInmuebleDTO>().ReverseMap();

            //LOCALIDAD - DTOs
            CreateMap<Localidad, LocalidadDTO>().ReverseMap();

            //INTERESMORA - DTOs
            CreateMap<InteresMora, InteresMoraDTO>().ReverseMap();

            //MUNICIPIO - DTOs
            CreateMap<Municipio, MunicipioDTO>().ReverseMap();
            CreateMap<Municipio, MunicipioListDTO>().ReverseMap();

            //MEDIOPAGO - DTOs
            CreateMap<MedioPago, MedioPagoDTO>().ReverseMap();

            //OBRAPUBLICA - DTOs
            CreateMap<ObraPublica, ObraPublicaDTO>().ReverseMap();

            //PAGOS - DTOs
            CreateMap<Pago, PagoDTO>().ReverseMap();

            //PLANPAGO - DTOs
            CreateMap<PlanPago, PlanPagoDTO>().ReverseMap();

            //PERMISO - DTOs
            CreateMap<Permiso, PermisoDTO>().ReverseMap();

            //USUARIO ROL - DTOs
            CreateMap<UsuarioRol, UsuarioRolDTO>().ReverseMap();

            //ContribuyentesImpuestosVariables - DTOs
            CreateMap<ContribuyentesImpuestosVariables, ContribuyentesImpuestosVariablesDTO>().ReverseMap();

        }
    }

}
