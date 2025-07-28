using FloristAI.Application.Store;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace FloristAI.Infrastructure
{
    public class GoogleSheets : IGoogleSheets
    {
        private readonly SheetsService _sheetsService;
        private readonly DriveService _driveService;

        public GoogleSheets(SheetsService sheetsService, DriveService driveService)
        {
            _sheetsService = sheetsService;
            _driveService = driveService;
        }

        /// <summary>
        /// Создает таблицу в Google Sheets и перемещает её в указанную папку.
        /// </summary>
        public async Task<string> CreateSpreadsheet(string name, string parentFolderId)
        {
            // 1. Создаем таблицу
            var spreadsheet = new Spreadsheet
            {
                Properties = new SpreadsheetProperties
                {
                    Title = name
                }
            };

            var createRequest = _sheetsService.Spreadsheets.Create(spreadsheet);
            var spreadsheetResponse = await createRequest.ExecuteAsync();
            var spreadsheetId = spreadsheetResponse.SpreadsheetId;

            // 2. Перемещаем таблицу в папку Google Drive
            if (!string.IsNullOrEmpty(parentFolderId))
            {
                var updateRequest = _driveService.Files.Update(new Google.Apis.Drive.v3.Data.File(), spreadsheetId);
                updateRequest.AddParents = parentFolderId;
                await updateRequest.ExecuteAsync();
            }

            return spreadsheetId;
        }

        /// <summary>
        /// Добавляет новый лист в существующую таблицу.
        /// </summary>
        public async Task AddSheet(string spreadsheetId, string sheetName)
        {
            var addSheetRequest = new Request
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = sheetName
                    }
                }
            };

            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request> { addSheetRequest }
            };

            var request = _sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId);
            await request.ExecuteAsync();
        }
    }
}
