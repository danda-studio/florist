using AutoMapper;
using FloristAI.Application.Store;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Application.Users.Models.Response;
using FloristAI.Core.Entities.ReferralsAndPartners;
using QRCoder;

namespace FloristAI.Application.Users
{
    public class PartnerService : IPartnerService
    {
        /// <summary>
        /// Репозитории для работы с данными пользователей.
        /// </summary>
        private readonly IPartnerRepository _partnerRepository;
        private readonly IUserRepository _userRepository;

        private readonly IStepFlowService _stepFlowService;
        private readonly IMapper _mapper;

        public PartnerService(IStepFlowService stepFlowService, IPartnerRepository partnerRepository, IUserRepository userRepository,IMapper mapper) 
        {
            _stepFlowService = stepFlowService;
            _partnerRepository = partnerRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<string> GetPartnerLink(GetPartnerLinkRequest request)
        {
            string botName = "FlowerKisaBot";

            string code = Guid.NewGuid().ToString("N")[..8];


            await AddPartnerFromInviteCode(new AddPartnerFromInviteCodeRequest
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.Phone,
                InviteCode = code
            });

            return $"https://t.me/{botName}?start=partner_{code}";
        }

        public async Task AddPartnerFromInviteCode(AddPartnerFromInviteCodeRequest request)
        {
            var partner = _mapper.Map<Partner>(request);
            partner.InviteCode = request.InviteCode;

            await _partnerRepository.AddPartner(partner);
        }

        public async Task RegisterPartner(long chatId, string spreadSheetId, string privateSpreadSheetId)
        {
            var stepData = await _stepFlowService.GetStep(chatId) ?? throw new InvalidOperationException($"Step data not found for chatId {chatId}");
            var request = new AddPartnerRequest
            {
                ChatId = stepData.ChatId,
                FirstName = stepData.FirstName ?? string.Empty,
                LastName = stepData.LastName ?? string.Empty,
                PhoneNumber = stepData.Phone ?? string.Empty,
                SpreadSheetId = spreadSheetId,
                PrivateSpreadSheetId = privateSpreadSheetId
            };

            await AddPartner(request);
            await _stepFlowService.ClearStep(chatId);
        }

        public async Task AddPartner(AddPartnerRequest request)
        {
            var user = await _userRepository.GetUserByChatId(request.ChatId) ?? throw new Exception("User not found");
            var partner = _mapper.Map<Partner>(request);
            partner.UserId = user.Id;

            await _partnerRepository.AddPartner(partner);
        }

        public byte[] GetPartnerLinkQrCode(string link)
        {
            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);

            var renderer = new PngByteQRCode(data);
            return renderer.GetGraphic(20);
        }

        public byte[] GetReferralQrCode(int id)
        {
            string link = GetReferralLink(id);

            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);

            var renderer = new PngByteQRCode(data);
            return renderer.GetGraphic(20);
        }

        public string GetReferralLink(int Id)
        {
            string botName = "FLowerKisaBot";
            return $"https://t.me/{botName}?start={Id}";
        }

        public async Task UpdatePartnerOnActivation(UpdatePartnerOnActivationRequest request)
        {
            var user = await _userRepository.GetUserByChatId(request.ChatId) ?? throw new Exception("User not found");
            var partner = _mapper.Map<Partner>(request);
            partner.InviteCode = request.InviteCode;
            partner.IsActive = true;

            await _partnerRepository.UpdatePartner(partner);
        }

        public async Task ProcessReferral(ProcessReferralRequest request)
        {

            var referal = new ProcessReferralModel
            {
                ReferalId = request.UserId,
                ProcessReferralItem = new ProcessReferralItem
                {
                    PartnerId = request.PartnerId,
                    ReferalId = request.UserId
                }
            };

            var referalEntity = _mapper.Map<Referal>(referal);

            await _partnerRepository.AddReferal(referalEntity, request.PartnerId);
        }

        public async Task<ResolvePartnerInviteResponse> ResolvePartnerInvite(string code)
        {
            var resolveInfo = await _partnerRepository.ResolvePartnerInvite(code);

            if (resolveInfo == null)
            {
                Console.WriteLine($"Не удалось разрешить приглашение партнёра по коду {code}");
                return new ResolvePartnerInviteResponse();
            }

            return new ResolvePartnerInviteResponse
            {
                PartnerId = resolveInfo.Id,
                FirstName = resolveInfo.FirstName,
                LastName = resolveInfo.LastName,
                Phone = resolveInfo.PhoneNumber,
                IsActive = resolveInfo.IsActive
            };
        }
    }
}
