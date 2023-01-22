using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<VillaDTO, Villa>();
            CreateMap<Villa, VillaDTO>();

            CreateMap<Villa, VillaCreateDTO>();
            CreateMap<VillaCreateDTO, Villa>();

            CreateMap<Villa, VillaUpdateDTO>();
            CreateMap<VillaUpdateDTO, Villa>();
        }
    }
}
