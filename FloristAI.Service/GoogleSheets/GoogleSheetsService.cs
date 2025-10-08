using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.GoogleSheets.Models.Response;
using FloristAI.Application.Language;
using FloristAI.Application.Store;
using FloristAI.Application.Store.Models.Response;
using FloristAI.Core.Store;
using Google.Apis.Sheets.v4.Data;
using System.Globalization;


namespace FloristAI.Application.GoogleSheets
{
    public class GoogleSheetsService : IGoogleSheetsService
    {
        private readonly IGoogleSheets _googleSheets;
        private readonly IUserRepository _userRepository;
        private readonly ILocalizationService _localizationService;

        public GoogleSheetsService(IGoogleSheets googleSheets, IUserRepository userRepository, ILocalizationService localizationService)
        {
            _googleSheets = googleSheets;
            _userRepository = userRepository;
            _localizationService = localizationService;
        }

        public async Task<decimal> GetMonthlyIncome(int userId)
        {
            var spreadsheetId = await _userRepository.GetSpreadsheetId(userId);
            if (string.IsNullOrEmpty(spreadsheetId))
                throw new InvalidOperationException("Spreadsheet ID not found for user");

            var values = await GetValues(spreadsheetId, "B1");

            if (values.Count == 0 || values[0].Count == 0)
                return 0m;

            if (decimal.TryParse(values[0][0]?.ToString(), out var total))
                return total;

            return 0m;
        }

        public async Task<GetModeratorSpreadsheetResponse> GetModeratorSpreadsheet(string spreadSheetName)
        {

            var spreadsheet = await _googleSheets.FindSpreadsheet(spreadSheetName, _localizationService.GetSheetName("RootFolderId"));
            if(spreadsheet.Success == false || spreadsheet.File == null)
            {
                var newSpreadsheet = await CreateSpreadsheetModerator(new CreateSpreadsheetModeratorRequest 
                { 
                    SheetName = spreadSheetName, 
                    FolderId = _localizationService.GetSheetName("RootFolderId")
                });

                return new GetModeratorSpreadsheetResponse
                {
                    Success = true,
                    SpreadSheetId = newSpreadsheet.SpreadsheetId,
                };
            }
            return new GetModeratorSpreadsheetResponse
            {
                Success = spreadsheet.Success,
                SpreadSheetId = spreadsheet.File.Id,
            };

        }

        public async Task<string> GetGoogleSheetsUrl(int userId)
        {
            var spreadsheetId = await _userRepository.GetSpreadsheetId(userId);
            if (string.IsNullOrEmpty(spreadsheetId))
                spreadsheetId = "0";

            return $"https://docs.google.com/spreadsheets/d/{spreadsheetId}";
        }

        public async Task<string> GetAdminGoogleSheetsUrl()
        {
            var spreadsheet = await _googleSheets.FindSpreadsheet(_localizationService.GetSheetName("General_Info"));
            if (!spreadsheet.Success || spreadsheet.File == null)
                throw new InvalidOperationException("Admin spreadsheet not found");

            return $"https://docs.google.com/spreadsheets/d/{spreadsheet.File.Id}";
        }

        public async Task<string> GetSheetIdByMonth(string spreadsheetId, DateTime date)
        {
            var sheets = await _googleSheets.GetSheets(spreadsheetId);
            var monthName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(date.ToString("MMMM"));

            var sheet = sheets.FirstOrDefault(s =>
                string.Equals(s.Properties.Title, monthName, StringComparison.OrdinalIgnoreCase));

            if (sheet == null)
            {
                // создаём новый лист
                await _googleSheets.AddSheet(spreadsheetId, monthName);

                // перечитываем список листов
                sheets = await _googleSheets.GetSheets(spreadsheetId);

                sheet = sheets.FirstOrDefault(s =>
                    string.Equals(s.Properties.Title, monthName, StringComparison.OrdinalIgnoreCase));

                if (sheet == null)
                    throw new Exception($"Не удалось создать лист для месяца '{monthName}'");
            }

            return sheet.Properties.Title;
        }


        public async Task<IList<IList<object>>> GetValues(string spreadsheetId, string range)
        {
            return await _googleSheets.GetValues(spreadsheetId, range);
        }

        public async Task<List<CreateStructureSheetResponse>> CreateStructureSheet(SheetsCreationParams parameters)
        {
            // Создаём все таблицы, но сохраняем ID публичной
            var spreadsheetsToCreate = new[]
            {
                (name: $"{parameters.PartnerId}/{parameters.FirstName} {parameters.LastName}/{DateTime.Now.Year}",
                 folderId: parameters.PublicFolderId,
                 IsPublic: true,
                 FlagName: "PublicPartnerInfo"),
                (name: $"{parameters.PartnerId}/{parameters.FirstName} {parameters.LastName}/{DateTime.Now.Year}",
                 folderId: parameters.PrivateFolderId,
                 IsPublic: false,
                 FlagName: "PrivatePartnerInfo"),
                (name: _localizationService.GetSheetName("General_Info"),
                 folderId: parameters.PrivateFolderId,
                 IsPublic: false,
                 FlagName: "TotalInfo")
            };

            var headersConfig = new Dictionary<string, List<string[]>>
            {
                {
                    "PublicPartnerInfo", new List<string[]>
                    {
                        new[] { _localizationService.GetSheetName("Total") },
                        new[]
                        {
                            _localizationService.GetSheetName("Date"),
                            _localizationService.GetSheetName("Income")
                        }
                    }
                },
                {
                    "PrivatePartnerInfo", new List<string[]>
                    {
                        new[] { _localizationService.GetSheetName("Total") },
                        new[]
                        {
                            _localizationService.GetSheetName("Date"),
                            _localizationService.GetSheetName("PartnerIncome"),
                            _localizationService.GetSheetName("NetIncome"),
                            _localizationService.GetSheetName("TotalIncome")
                        }
                    }
                },
                {
                    "TotalInfo", new List<string[]>
                    {
                        new[] { _localizationService.GetSheetName("Total") },
                        new[]
                        {
                            _localizationService.GetSheetName("Id"),
                            _localizationService.GetSheetName("FullName"),
                            _localizationService.GetSheetName("PhoneNumber"),
                            _localizationService.GetSheetName("TelegramId"),
                            _localizationService.GetSheetName("TelegramUsername"),
                            _localizationService.GetSheetName("ReferralCount"),
                            _localizationService.GetSheetName("PartnerTotalIncome"),
                            _localizationService.GetSheetName("NetTotalIncome"),
                            _localizationService.GetSheetName("GrandTotalIncome")
                        }
                    }
                }
            };

            var result = new List<CreateStructureSheetResponse>();
            var monthSheetName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Now.ToString("MMMM"));

            foreach (var spreadsheet in spreadsheetsToCreate)
            {
                // Проверяем, существует ли уже таблица
                var sheetInfo = await CreateSpreadsheet(spreadsheet.name, spreadsheet.folderId);
                if (sheetInfo.IsNew == false)
                {
                    // Получаем текущую информацию о листах таблицы
                    var spreadsheetData = await _googleSheets.GetSheets(sheetInfo.SpreadsheetId);

                    // Проверяем, есть ли лист месяца
                    bool monthSheetExists = spreadsheetData.Any(s => s.Properties.Title == monthSheetName);

                    if (!monthSheetExists)
                    {
                        await _googleSheets.AddSheet(sheetInfo.SpreadsheetId, monthSheetName);
                    }

                    if (headersConfig.TryGetValue(spreadsheet.FlagName, out var existingTableSheetHeaders))
                    {
                        int maxColumns = existingTableSheetHeaders.Max(h => h.Length);
                        int rows = existingTableSheetHeaders.Count;

                        var lastColumn = GetColumnLetter(maxColumns);
                        var range = $"{monthSheetName}!A1:{lastColumn}{rows}";

                        await _googleSheets.AddHeaders(sheetInfo.SpreadsheetId, range, existingTableSheetHeaders);
                    }

                    result.Add(new CreateStructureSheetResponse
                    {
                        SpreadsheetId = sheetInfo.SpreadsheetId,
                        IsPublic = spreadsheet.IsPublic,
                        FileName = spreadsheet.name,
                        SheetName = monthSheetName
                    });

                    continue;
                }


                await _googleSheets.AddSheet(sheetInfo.SpreadsheetId, monthSheetName);


                if (headersConfig.TryGetValue(spreadsheet.FlagName, out var headers))
                {
                    int maxColumns = headers.Max(h => h.Length);
                    int rows = headers.Count;

                    var lastColumn = GetColumnLetter(maxColumns);
                    var range = $"{monthSheetName}!A1:{lastColumn}{rows}";

                    await _googleSheets.AddHeaders(sheetInfo.SpreadsheetId, range, headers);

                    if (sheetInfo.IsNew)
                    {
                        await _googleSheets.AddSheet(sheetInfo.SpreadsheetId, _localizationService.GetSheetName("Total"));
                        range = $"{_localizationService.GetSheetName("Total")}!A1:{lastColumn}{rows}";
                        await _googleSheets.AddHeaders(sheetInfo.SpreadsheetId, range, headers);
                    }
                }


                await DeleteDefaultSheet(sheetInfo.SpreadsheetId);
                result.Add(new CreateStructureSheetResponse
                {
                    SpreadsheetId = sheetInfo.SpreadsheetId,
                    IsPublic = spreadsheet.IsPublic,
                    FileName = spreadsheet.name,
                    SheetName = monthSheetName
                });
            }

            if (result.Count == 0)
            {
                throw new InvalidOperationException("Не удалось создать или найти ни одной таблицы");
            }

            return result;
        }


        public async Task<CreateSpreadsheetModeratorResponse> CreateSpreadsheetModerator(CreateSpreadsheetModeratorRequest request)
        {
            var headers = new List<string[]>
            {
                new[] 
                { 
                    _localizationService.GetSheetName("TelegramId"), 
                    _localizationService.GetSheetName("FullName") 
                }
            };

            // Создаём таблицу
            var sheetInfo = await CreateSpreadsheet(request.SheetName, request.FolderId);

            if (sheetInfo.IsNew != false)
            {
                int maxColumns = headers.Max(h => h.Length);
                int rows = headers.Count;
                var lastColumn = GetColumnLetter(maxColumns);
                var range = $"Лист1!A1:{lastColumn}{rows}";

                await _googleSheets.AddHeaders(sheetInfo.SpreadsheetId, range, headers);
            }

            return new CreateSpreadsheetModeratorResponse
            {
                SheetName = request.SheetName,
                SpreadsheetId = sheetInfo.SpreadsheetId
            };
        }

        public async Task<CreateSpreadsheetResponse> CreateSpreadsheet(string name, string parentFolderId)
        {
            var sheet = await _googleSheets.CreateSpreadsheet(name, parentFolderId);
            return new CreateSpreadsheetResponse
            {
                SpreadsheetId = sheet.SpreadsheetId,
                IsNew = sheet.IsNew
            };
        }

        public async Task AddSheet(string spreadSheetId, string sheetName)
        {
            await _googleSheets.AddSheet(spreadSheetId, sheetName);
        }

        public async Task<AddDataInRowResponse> AddDataInRow(AddDataRequest request)
        {
            await _googleSheets.AddData(request);

            return new AddDataInRowResponse
            {
                UserId = request.UserId,
                SpreadsheetId = request.SpreadsheetId,
                Success = true
            };
        }

        public Task UpdateValue(string spreadsheetId, string range, string value)
        {
            return _googleSheets.UpdateValue(spreadsheetId, range, value);
        }

        public async Task DeleteDefaultSheet(string spreadsheetId)
        {
            if (string.IsNullOrEmpty(spreadsheetId))
                throw new ArgumentException("Spreadsheet ID cannot be null or empty.", nameof(spreadsheetId));
            try
            {
                await _googleSheets.DeleteDefaultSheet(spreadsheetId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete default sheet.", ex);
            }
        }

        public async Task<FindSpreadsheetResponse> FindSpreadsheet(string name)
        {
            var spreadsheet = await _googleSheets.FindSpreadsheet(name);
            return new FindSpreadsheetResponse 
            {
                Success = spreadsheet.Success,
                File = spreadsheet.File
            };
        }

        private static string GetColumnLetter(int columnIndex)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = string.Empty;

            while (columnIndex > 0)
            {
                columnIndex--;
                result = letters[columnIndex % 26] + result;
                columnIndex /= 26;
            }

            return result;
        }

    }

}
