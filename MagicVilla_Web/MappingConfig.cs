using AutoMapper;
using MagicVilla_Web.Models.DTO;

namespace MagicVilla_Web
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
            CreateMap<VillaDTO,VillaCreateDTO>();
            CreateMap<VillaCreateDTO, VillaDTO>();

            // Second way create mapping reversible 
            CreateMap<VillaDTO, VillaUpdateDTO>();
            CreateMap<VillaUpdateDTO, VillaDTO>();

            CreateMap<VillaNumberDTO, VillaNumberCreateDTO>().ReverseMap();
            CreateMap<VillaNumberDTO, VillaNumberUpdateDTO>().ReverseMap();

            //Add service to program.cs
        }
    }
}
