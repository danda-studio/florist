using FloristAI.Core.Entities.ReferralsAndPartners;

namespace FloristAI.Application.Store
{
    public interface IPartnerRepository
    {
        Task<Partner?> GetPartnerByInviteCode(string inviteCode);
        Task<Partner?> ResolvePartnerInvite(string code);
        Task<Partner> AddPartner(Partner partner);
        Task<Referal> AddReferal(Referal referal, int partnerId);
        Task UpdatePartner(Partner partner);
        Task<bool> IsPartner(long chatId);
    }
}
