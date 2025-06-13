using AutoMapper;
using CrudDashboard.Dto;
using CrudDashboard.Models;

namespace CrudDashboard.Profiles
{
    public class ProfileAutoMapper : Profile
    {
        public ProfileAutoMapper() 
        { 
            //De Usuario para UsuarioListarDTO.
            CreateMap<Conta, ContaDto>();
            CreateMap<Movimentacao, MovimentacoesCadastrarDto>();
            CreateMap<Movimentacao, MovimentacoesDto>();
            CreateMap<Movimentacao, MovimentacoesConferidoDto>();

            //Caso quisesse transformar UsuarioListarDto para Usuario.
            //CreateMap<Usuario, UsuarioListarDto>().ReverseMap();
        }
    }
}
