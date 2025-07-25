using FloristAI.Application.GoogkeSheets;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Infrastructure
{
    public class GoogleSheetsService : IGoogleSheetsService
    {

        private readonly SheetsService _googleSheetsService;
        private readonly string _spreadsheetId;

        public GoogleSheetsService(SheetsService googleSheetsService, string spreadsheetId)
        {
            _googleSheetsService = googleSheetsService;
            _spreadsheetId = spreadsheetId;
        }

        public async Task<decimal> GetMonthlyIncome(int userId)
        {

            throw new NotImplementedException();
        }

        public async Task<string> GetGoogleSheetsUrl(int userId)
        {

            return $"https://docs.google.com/spreadsheets/d/{_spreadsheetId}/edit#gid={userId}";
        }

        public async Task<string> CreateFolder(string FirstName, string LastName, int userId)
        {
            return $"https://drive.google.com/drive/folders/{userId}";
        }

        public async Task<string> CreateSpreadsheet()
        {
            return $"https://docs.google.com/spreadsheets/d/{_spreadsheetId}/edit";
        }
    }
}
