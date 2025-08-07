using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.Store;
using FloristAI.Application.Store.Models.Response;
using Google;
using Google.Apis.Drive.v3;
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
        public async Task<CreateSpreadsheetResponse> CreateSpreadsheet(string name, string parentFolderId)
        {
            try
            {
                var (success, existingFile) = await FindSpreadsheet(name, parentFolderId);

                if (success && existingFile != null)
                {
                    return new CreateSpreadsheetResponse
                    { 
                        SpreadsheetId = existingFile.Id,
                        IsNew = false,
                    };

                }

                await Task.Delay(1500);

                //var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                //{
                //    Name = name,
                //    MimeType = "application/vnd.google-apps.spreadsheet",
                //    Parents = new List<string> { parentFolderId }
                //};

                //var createRequest = _driveService.Files.Create(fileMetadata);
                //createRequest.SupportsAllDrives = true; 
                //var file = await createRequest.ExecuteAsync();
                //return file.Id;


                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = name,
                    MimeType = "application/vnd.google-apps.spreadsheet",
                    Parents = new List<string> { parentFolderId }
                };

                var request = _driveService.Files.Create(fileMetadata);
                request.SupportsAllDrives = true;
                var file = await request.ExecuteAsync();
                return new CreateSpreadsheetResponse
                {
                    SpreadsheetId = file.Id,
                    IsNew = true,
                };
            }
            catch (GoogleApiException ex) when (ex.Error.Code == 403)
            {
                throw new Exception($"Ошибка доступа (403). Детали:\n" +
                                   $"• Сообщение ошибки: {ex.Message}\n");
            }
            catch (Exception ex)
            {
                throw new Exception($"Неожиданная ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Добавляет новый лист в существующую таблицу.
        /// </summary>
        public async Task<string> AddSheet(string spreadsheetId, string sheetName)
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

            return request.SpreadsheetId;
        }

        /// <summary>
        /// Поиск таблицы по имени и родительской папке
        /// </summary>
        private async Task<(bool Success, Google.Apis.Drive.v3.Data.File? File)> FindSpreadsheet(string name, string parentFolderId)
        {
            try
            {
                var listRequest = _driveService.Files.List();
                listRequest.Q = $"name = '{name}' and '{parentFolderId}' in parents and mimeType = 'application/vnd.google-apps.spreadsheet' and trashed = false";
                listRequest.SupportsAllDrives = true;
                listRequest.IncludeItemsFromAllDrives = true;
                listRequest.Fields = "files(id, name)";

                var result = await listRequest.ExecuteAsync();
                var file = result.Files.FirstOrDefault();

                return file != null
                    ? (true, file)
                    : (false, null);
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем выполнение
                Console.WriteLine($"{ex}, Ошибка при поиске таблицы {name} в папке {parentFolderId}");
                return (false, null);
            }
        }


        public async Task AddHeaders(string spreadsheetId, string range, List<string[]> headers)
        {
            if (headers == null || headers.Count == 0)
                throw new ArgumentException("Заголовки не могут быть пустыми.", nameof(headers));

            var values = headers
                .Select(row => row.Cast<object>().ToList())
                .ToList<IList<object>>();

            var valueRange = new ValueRange
            {
                Range = range,
                Values = values
            };

            var updateRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            await updateRequest.ExecuteAsync();
        }

        public async Task AddData(AddDataRequest request)
        {

            var rowValues = new List<object>
            {
                request.UserId,
                request.UserData.NameAndSurname,
                request.UserData.PhoneNumber,
                request.UserData.TelegramId,
                request.UserData.TelegramUsername,
                request.UserData.CountReferals,
                request.UserData.TotalPartnerIncome,
                request.UserData.TotalIncomeWithoutСommissionPartner,
                request.UserData.TotalIncome,
            };

            var valueRange = new ValueRange
            {
                Values = new List<IList<object>> { rowValues }
            };

            var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, request.SpreadsheetId, $"{request.SheetName}!A2");
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            appendRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;

            await appendRequest.ExecuteAsync();
        }


        public async Task<IList<IList<object>>> GetValues(string spreadsheetId, string range)
        {
            var request = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = await request.ExecuteAsync();
            return response.Values ?? new List<IList<object>>();
        }
    }
}
