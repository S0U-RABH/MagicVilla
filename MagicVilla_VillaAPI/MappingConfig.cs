using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI
{
    //Auto mapper is liye use krte he jab object me bohot sari field hoti he to ek ek ko map krne me time 
    // lagega is liye auto mapper ka use krte he vo auto map kr deta he automatic sab ko.

    /*AutoMapper is a simple library that helps us to transform one object type into another. 
     * It is a convention-based object-to-object mapper that requires very little configuration. 
     * The object-to-object mapping works by transforming an 
     * input object of one type into an output object of a different type.
     */
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            // first way create mapping and then make reverse of it.
            CreateMap<Villa,VillaDTO>();
            CreateMap<VillaDTO, Villa>();

            // Second way create mapping reversible 
            CreateMap<Villa,VillaCreateDTO>().ReverseMap();
            CreateMap<Villa, VillaUpdateDTO>().ReverseMap();

            //Add service to program.cs 
            // after that configer mapping in ApiController.

            CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();
        }
    }
}
