using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleSheets.Models.Request
{
    public record SheetsCreationParams(
    int? PartnerId,
    string? FirstName,
    string? LastName,
    string PublicFolderId,
    string PrivateFolderId);
}
