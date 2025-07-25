using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogkeSheets
{
    public interface IGoogleSheetsService
    {
        Task<decimal> GetMonthlyIncome(int userId);
        Task<string> GetGoogleSheetsUrl(int userId);
        Task<string> CreateFolder(string FirstName, string LastName, int userId);
        Task<string> CreateSpreadsheet();
    }
}
