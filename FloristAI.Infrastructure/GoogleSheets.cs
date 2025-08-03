using FloristAI.Application.Store;
using Google;
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
            try
            {
                var (success, existingFile) = await FindSpreadsheet(name, parentFolderId);

                if (success && existingFile != null)
                {
                    return existingFile.Id;
                }

                await Task.Delay(1500);
                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = name,
                    MimeType = "application/vnd.google-apps.spreadsheet",
                    Parents = new List<string> { parentFolderId }
                };

                var request = _driveService.Files.Create(fileMetadata);
                request.SupportsAllDrives = true;
                var file = await request.ExecuteAsync();
                return file.Id;
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

        /// <summary>
        /// Поиск таблицы по имени и родительской папке
        /// </summary>
        private async Task<(bool Success, Google.Apis.Drive.v3.Data.File? File)> FindSpreadsheet(string name, string parentFolderId)
        {
            try
            {
                var listRequest = _driveService.Files.List();
                listRequest.Q = $"name = '{name}' and '{parentFolderId}' in parents and mimeType = 'application/vnd.google-apps.spreadsheet'";
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

        public async Task<IList<IList<object>>> GetValues(string spreadsheetId, string range)
        {
            var request = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = await request.ExecuteAsync();
            return response.Values ?? new List<IList<object>>();
        }
    }
}
