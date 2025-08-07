using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleSheets.Models.Request
{
    public class UserData
    {
        public string LinkOnTheSheetUser { get; set; } = string.Empty;
        public string NameAndSurname { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public long TelegramId { get; set; }
        public string TelegramUsername { get; set; } = string.Empty;
        public int CountReferals { get; set; }
        public double TotalPartnerIncome { get; set; }
        public double TotalIncomeWithoutСommissionPartner { get; set; }

        public double TotalIncome { get; set; }
    }
}
