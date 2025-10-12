using AutoMapper;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Application.Users.Models.Response;
using FloristAI.Core.Entities.ReferralsAndPartners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Mapping
{
    public class MappingReferalProfile : Profile
    {
        public MappingReferalProfile()
        {
            // Маппинг для Referal
            CreateMap<ProcessReferralModel, Referal>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ReferalId))
                .ForMember(dest => dest.PartnerReferal, opt => opt.MapFrom(src => src.ProcessReferralItem)); // вложенная модель

            // Маппинг вложенной модели
            CreateMap<ProcessReferralItem, PartnerReferal>()
                .ForMember(dest => dest.PartnerId, opt => opt.MapFrom(src => src.PartnerId))
                .ForMember(dest => dest.ReferalId, opt => opt.MapFrom(src => src.ReferalId));
        }
    }
}
