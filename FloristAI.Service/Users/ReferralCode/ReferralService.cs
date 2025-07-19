using QRCoder;
using System;

namespace FloristAI.Application.Users.ReferralCode
{
    public class ReferralService : IReferralService
    {
        private readonly IUserService _userService;

        public ReferralService(IUserService userService)
        {
            _userService = userService;
        }

        public string GetReferralLink(int Id)
        {
            string botName = "FLowerKisaBot";
            return $"https://t.me/{botName}?start={Id}";
        }

        public byte[] GetReferralQrCode(int id)
        {
            string link = GetReferralLink(id);

            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);

            var renderer = new PngByteQRCode(data);
            return renderer.GetGraphic(20);
        }
    }
}