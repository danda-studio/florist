using FloristAI.Application.Users.Models.Request;
using FloristAI.Application.Users.Models.Response;
using FloristAI.Core.Entities.ReferralsAndPartners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Users
{
    public interface IPartnerService
    {
        Task<string> GetPartnerLink(GetPartnerLinkRequest request);
        Task AddPartnerFromInviteCode(AddPartnerFromInviteCodeRequest request);
        Task<ResolvePartnerInviteResponse> ResolvePartnerInvite(string code);
        Task RegisterPartner(long chatId, string spreadSheetId, string privateSpreadSheetId);
        Task AddPartner(AddPartnerRequest request);
        Task UpdatePartnerOnActivation(UpdatePartnerOnActivationRequest request);
        Task ProcessReferral(ProcessReferralRequest request);
        byte[] GetPartnerLinkQrCode(string link);
        byte[] GetReferralQrCode(int id);
        string GetReferralLink(int Id);

    }
}
