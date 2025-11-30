using AutoMapper;
using FloristAI.Application.Boutique.Model.Request;
using FloristAI.Core.Entities;

namespace FloristAI.Application.Mapping
{
    public class MappingShopProfile: Profile
    {
        public MappingShopProfile() 
        {
            CreateMap<ShopModel, Shop>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            
            //CreateMap<AddShopsRequest, Shop>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude)) 
            //    .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
            //    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            //    .ForMember(dest => dest.UrlGoogleMap, opt => opt.MapFrom(src => src.UrlGoogleMap));
        }
        
    }
}
