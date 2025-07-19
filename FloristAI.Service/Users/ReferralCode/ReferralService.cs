using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

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
            // Генерация реферальной ссылки
            return $"https://t.me/{botName}?start={Id}";

        }


    }
}
