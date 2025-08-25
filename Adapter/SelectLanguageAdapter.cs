using FloristAI.Adapter.Models;
using FloristAI.Application.GoogleSheets;
using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter
{
    /// <summary>
    /// Адаптер обработки команды выбора языка интерфейса Telegram-бота.
    /// </summary>
    public class SelectLanguageAdapter : IMessageAdapter
    {
        /// <summary>
        /// Сервис для получения списка доступных языков.
        /// </summary>
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;

        private readonly IUserService _userService;
        
        private readonly IGoogleSheetsService _googleSheetsService;

        /// <summary>
        /// Ключ маршрута, соответствующий команде.
        /// </summary>
        public string RouteKey => "start";

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SelectLanguageAdapter"/>.
        /// </summary>
        /// <param name="languageService">Сервис для получения списка поддерживаемых языков.</param>
        public SelectLanguageAdapter(ILanguageService languageService, IUserService userService, IGoogleSheetsService googleSheetsService, ILocalizationService localizationService)
        {
            _languageService = languageService;
            _userService = userService;
            _googleSheetsService = googleSheetsService;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Обрабатывает сообщение и возвращает список языков в виде инлайн-кнопок.
        /// </summary>
        /// <param name="message">Входящее сообщение пользователя.</param>
        /// <param name="chatId">Идентификатор чата, откуда пришло сообщение.</param>
        /// <returns>
        /// Объект <see cref="MessageResult"/>, содержащий текст и клавиатуру с выбором языка.
        /// </returns>
        public async Task<List<MessageResult>> ProcessMessage(MessageContext context)
        {
            var user = await _userService.GetOrCreateUser(context.ChatId, "ru");

            int partnerId = 0;
            if (!string.IsNullOrEmpty(context.Parameter) && !int.TryParse(context.Parameter, out _))
            {
                var partnerInfo = await _userService.ResolvePartnerInvite(context.Parameter);

                var request = new CreateStructureFolderAndSheetRequest
                {
                    PartnerId = partnerInfo.PartnerId,
                    FirstName = partnerInfo.FirstName,
                    LastName = partnerInfo.LastName,
                };

                var sheet = await _userService.CreateStructureFolderAndSheet(request);

                var SheetId = sheet.FirstOrDefault(s => s.FileName == _localizationService.GetString("Total_Info", "sheetName") || s.SheetName == _localizationService.GetString("Total_Info", "sheetName")) ?? throw new Exception("Не удалось найти таблицу");
                var publicSheet = sheet.FirstOrDefault(s => s.IsPublic == true) ?? throw new Exception("Не удалось найти публичную таблицу");

                await _userService.UpdatePartnerOnActivation(context.ChatId, publicSheet.SpreadsheetId, context.Parameter);

                await _userService.AddDataInRow(
                    new AddDataRequest
                    {
                        UserId = user.UserId,
                        SpreadsheetId = SheetId.SpreadsheetId,
                        SheetName = SheetId.SheetName,
                        UserData = new UserData
                        {
                            NameAndSurname = $"{partnerInfo.FirstName} {partnerInfo.LastName}",
                            PhoneNumber = partnerInfo.Phone,
                            TelegramId = context.ChatId,
                            TelegramUsername = context.Username ?? string.Empty
                        }
                    });
            }

            if (int.TryParse(context.Parameter, out partnerId))
            {
                await _userService.ProcessReferral(new ProcessReferralRequest
                {
                    UserId = user.UserId,
                    PartnerId = partnerId
                });
            }
        
            

            var spreadsheetId = await _googleSheetsService.FindSpreadsheet(_localizationService.GetString("Total_Info", "sheetName"));
            if (!string.IsNullOrEmpty(spreadsheetId))
            {
                var sheetName = await _googleSheetsService.GetSheetIdByMonth(spreadsheetId, DateTime.Now);

                var rows = await _googleSheetsService.GetValues(spreadsheetId, $"{sheetName}!A2:I");
                for (int i = 0; i < rows.Count; i++)
                {
                    if (rows[i][0]?.ToString() == partnerId.ToString())
                    {
                        int currentCount = 0;
                        if (rows[i].Count > 5 && int.TryParse(rows[i][5]?.ToString(), out var count))
                            currentCount = count;

                        currentCount++;

                        await _googleSheetsService.UpdateValue(
                            spreadsheetId,
                            $"{sheetName}!F{i + 2}", 
                            currentCount.ToString()
                        );
                    }
                }
                
            }

            var language = await _languageService.GetLanguageList(context.ChatId);

            var keyboard = new InlineKeyboardMarkup(
                language.Select(lang =>
                    new[] {
                        InlineKeyboardButton.WithCallbackData(
                            text: lang.Name,
                            callbackData: $"select_role:{lang.Code}"
                        )
                    }).ToArray()
            );

            return new List<MessageResult>
            {
                new MessageResult
                {
                    Text = "🌐 Выберите язык / Selectați o limbă",
                    ReplyMarkup = keyboard
                }
            };
            
        }
    }
}
