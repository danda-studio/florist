using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Store
{
    public interface IGoogleSheets
    {
        Task<decimal> GetMonthlyIncome(int userId);
        Task<string> GetGoogleSheetsUrl(int userId);
        Task<string> CreateFolder(string FirstName, string LastName, int userId);
        Task<string> CreateSpreadsheet();
    }
}
