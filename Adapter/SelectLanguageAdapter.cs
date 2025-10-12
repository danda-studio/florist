using FloristAI.Adapter.Models;
using FloristAI.Application;
using FloristAI.Application.GoogleDrive;
using FloristAI.Application.GoogleSheets;
using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
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
        private readonly IUserService _userService;
        private readonly IPartnerService _partnerService;

        private readonly IGoogleSheetsService _googleSheetsService;
        private readonly IGoogleDriveService _googleDriveService;

        private readonly IPinnedMessageService _pinnedMessageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Ключ маршрута, соответствующий команде.
        /// </summary>
        public string RouteKey => "start";

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SelectLanguageAdapter"/>.
        /// </summary>
        /// <param name="languageService">Сервис для получения списка поддерживаемых языков.</param>
        public SelectLanguageAdapter(
            IUserService userService,
            IPartnerService partnerService,
            IGoogleSheetsService googleSheetsService,
            IGoogleDriveService googleDriveService,
            ILocalizationService localizationService,
            IPinnedMessageService pinnedMessageService,
            ILanguageService languageService)
        {
            _userService = userService;
            _partnerService = partnerService;
            _googleDriveService = googleDriveService;
            _googleSheetsService = googleSheetsService;
            _languageService = languageService;
            _localizationService = localizationService;
            _pinnedMessageService = pinnedMessageService;
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
            var user = await _userService.GetOrCreateUser(context.ChatId, "ru", false);

            int partnerId = 0;
            if (!string.IsNullOrEmpty(context.Parameter) && !int.TryParse(context.Parameter, out _))
            {
                var partnerInfo = await _partnerService.ResolvePartnerInvite(context.Parameter);
                if (partnerInfo.IsActive != true)
                {
                    var folder = await _googleDriveService.CreateStructureFolder();

                    var request = new SheetsCreationParams(
                        PartnerId: partnerInfo.PartnerId,
                        FirstName: partnerInfo.FirstName,
                        LastName: partnerInfo.LastName,
                        PrivateFolderId: folder.PrivateFolderId,
                        PublicFolderId: folder.PublicFolderId
                    );

                    var sheet = await _googleSheetsService.CreateStructureSheet(request);

                    var SheetId = sheet.FirstOrDefault(s => s.FileName == _localizationService.GetSheetName("General_Info") || s.SheetName == _localizationService.GetSheetName("General_Info")) ?? throw new Exception("Не удалось найти таблицу");
                    var privateSheet = sheet.FirstOrDefault(s => s.FileName != _localizationService.GetSheetName("General_Info") && s.IsPublic == false) ?? throw new Exception("Не удалось найти приватную таблицу");
                    var publicSheet = sheet.FirstOrDefault(s => s.IsPublic == true) ?? throw new Exception("Не удалось найти публичную таблицу");

                    await _partnerService.UpdatePartnerOnActivation(new UpdatePartnerOnActivationRequest
                    {
                        ChatId = context.ChatId,
                        SpreadSheetId = publicSheet.SpreadsheetId,
                        PrivateSpreadSheetId = privateSheet.SpreadsheetId,
                        InviteCode = context.Parameter
                    });

                    await _googleSheetsService.AddDataInRowPartnerTable(
                        new AddDataRequest
                        {
                            UserId = user.UserId,
                            SpreadsheetId = SheetId.SpreadsheetId,
                            PrivateSpreadsheetId = privateSheet.SpreadsheetId,
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
            }

            if (int.TryParse(context.Parameter, out partnerId))
            {
                await _partnerService.ProcessReferral(new ProcessReferralRequest
                {
                    UserId = user.UserId,
                    PartnerId = partnerId
                });

                var spreadsheet = await _googleSheetsService.FindSpreadsheet(_localizationService.GetSheetName("General_Info"));
                if (!string.IsNullOrEmpty(spreadsheet.File.Id))
                {
                    var sheetName = await _googleSheetsService.GetSheetIdByMonth(spreadsheet.File.Id, DateTime.Now);

                    var rows = await _googleSheetsService.GetValues(spreadsheet.File.Id, $"{sheetName}!A2:I");
                    for (int i = 0; i < rows.Count; i++)
                    {
                        if (rows[i][0]?.ToString() == partnerId.ToString())
                        {
                            int currentCount = 0;
                            if (rows[i].Count > 5 && int.TryParse(rows[i][5]?.ToString(), out var count))
                                currentCount = count;

                            currentCount++;

                            await _googleSheetsService.UpdateValue(
                                spreadsheet.File.Id,
                                $"{sheetName}!F{i + 2}",
                                currentCount.ToString()
                            );
                        }
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

            //При деплое удалить из проекта
            var bytes = await File.ReadAllBytesAsync("images/коть.jpg");

            if (!_pinnedMessageService.HasPermanentMessage(context.ChatId))
            {
                _pinnedMessageService.SetPermanentMessage(context.ChatId);

                return new List<MessageResult>
                {
                    new MessageResult
                    {
                        Photo = new PhotoContent
                        {
                            ImageBytes = bytes
                        },
                        Text = "Купи ей цветы 💐 — маленький жест, который скажет больше, чем тысяча слов. Не откладывай, сделай это сегодня!",
                        PinnedMessage = true
                    },
                    new MessageResult
                    {
                        Text = "🌐 Выберите язык / Selectați o limbă",
                        ReplyMarkup = keyboard
                    }
                };
            }
            else
            {
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
}
