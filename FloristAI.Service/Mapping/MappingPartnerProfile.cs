using AutoMapper;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Core.Entities.ReferralsAndPartners;

namespace FloristAI.Application.Mapping
{
    public class MappingPartnerProfile : Profile
    {
        public MappingPartnerProfile()
        {
            CreateMap<AddPartnerFromInviteCodeRequest, Partner>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())           
                .ForMember(dest => dest.InviteCode, opt => opt.Ignore())    // InviteCode задаём отдельно
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.Partners, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

            CreateMap<AddPartnerRequest, Partner>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.SpreadsheetId, opt => opt.MapFrom(src => src.SpreadSheetId))    // InviteCode задаём отдельно
                .ForMember(dest => dest.PrivateSpreadsheetId, opt => opt.MapFrom(src => src.PrivateSpreadSheetId))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));
        }
    }
}
