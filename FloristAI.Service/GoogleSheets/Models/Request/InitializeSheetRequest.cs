
namespace FloristAI.Application.GoogleSheets.Models.Request
{
    public class InitializeSheetRequest
    {
        public string FolderId { get; set; } = string.Empty;

        public string SheetName { get; set; } = string.Empty;
    }
}
