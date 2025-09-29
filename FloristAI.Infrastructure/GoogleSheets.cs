using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.Language;
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
        private readonly ILocalizationService _localizationService;

        public GoogleSheets(SheetsService sheetsService, DriveService driveService, ILocalizationService localizationService)
        {
            _sheetsService = sheetsService;
            _driveService = driveService;
            _localizationService = localizationService;
        }

        public async Task<IList<IList<object>>> GetValues(string spreadsheetId, string range)
        {
            var request = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = await request.ExecuteAsync();
            return response.Values ?? new List<IList<object>>();
        }

        public async Task<IList<Sheet>> GetSheets(string spreadsheetId)
        {
            var request = _sheetsService.Spreadsheets.Get(spreadsheetId);
            var response = await request.ExecuteAsync();
            return response.Sheets;
        }

        /// <summary>
        /// Добавляет новый лист в существующую таблицу.
        /// </summary>
        public async Task<string> AddSheet(string spreadsheetId, string sheetName)
        {
            var spreadsheet = await _sheetsService.Spreadsheets.Get(spreadsheetId).ExecuteAsync();
            var sheets = spreadsheet.Sheets;

            var totalSheet = sheets.FirstOrDefault(s => s.Properties.Title.Equals(_localizationService.GetSheetName("Total"), StringComparison.OrdinalIgnoreCase));

            int insertIndex;
            if (totalSheet != null)
            {
                // Вставляем новый лист перед "ИТОГО"
                insertIndex = totalSheet.Properties.Index ?? sheets.Count;
            }
            else
            {
                // Если "ИТОГО" нет — добавляем в конец
                insertIndex = sheets.Count;
            }

            var addSheetRequest = new Request
            {
                AddSheet = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = sheetName,
                        Index = insertIndex
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
            // Формируем ссылку для userId
            var privateSpreadsheetId = request.PrivateSpreadsheetId; 
            var hyperlinkFormula = $"=ГИПЕРССЫЛКА(\"https://docs.google.com/spreadsheets/d/{privateSpreadsheetId}\";\"{request.UserId}\")";

            var rowValues = new List<object>
            {
                hyperlinkFormula,
                request.UserData.NameAndSurname,
                $"'{request.UserData.PhoneNumber}",
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
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            appendRequest.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;

            await appendRequest.ExecuteAsync();
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

        public async Task UpdateValue(string spreadsheetId, string range, string value)
        {
            var body = new ValueRange
            {
                Values = new List<IList<object>> { new List<object> { value } }
            };

            var request = _sheetsService.Spreadsheets.Values.Update(body, spreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            await request.ExecuteAsync();
        }

        public async Task DeleteDefaultSheet(string spreadsheetId)
        {
            // Получаем ID первого листа
            var spreadsheet = await _sheetsService.Spreadsheets.Get(spreadsheetId).ExecuteAsync();
            var sheetId = spreadsheet.Sheets.First().Properties.SheetId;

            // Формируем запрос на удаление
            var requestBody = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>
                {
                    new Request
                    {
                        DeleteSheet = new DeleteSheetRequest
                        {
                            SheetId = sheetId
                        }
                    }
                }
            };

            // Отправляем запрос
            var deleteRequest = _sheetsService.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
            await deleteRequest.ExecuteAsync();
        }
        
        /// <summary>
        /// Поиск таблицы по имени и родительской папке
        /// </summary>
        public async Task<(bool Success, Google.Apis.Drive.v3.Data.File? File)> FindSpreadsheet(string name, string? parentFolderId = null)
        {
            try
            {
                // Защита от одиночных кавычек в имени
                var safeName = (name ?? string.Empty).Replace("'", "\\'");

                // Формируем запрос в зависимости от наличия parentFolderId
                string query = $"name = '{safeName}' and mimeType = 'application/vnd.google-apps.spreadsheet' and trashed = false";
                if (!string.IsNullOrEmpty(parentFolderId))
                {
                    query = $"name = '{safeName}' and '{parentFolderId}' in parents and mimeType = 'application/vnd.google-apps.spreadsheet' and trashed = false";
                }

                var listRequest = _driveService.Files.List();
                listRequest.Q = query;
                listRequest.SupportsAllDrives = true;
                listRequest.IncludeItemsFromAllDrives = true;
                listRequest.Fields = "files(id, name)";

                var result = await listRequest.ExecuteAsync();
                var file = result.Files?.FirstOrDefault();

                return file != null ? (true, file) : (false, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}, Ошибка при поиске таблицы '{name}'{(string.IsNullOrEmpty(parentFolderId) ? "" : $" в папке {parentFolderId}")}");
                return (false, null);
            }
        }

    }
}
