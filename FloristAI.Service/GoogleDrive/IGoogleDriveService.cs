using FloristAI.Application.GoogleDrive.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleDrive
{
    public interface IGoogleDriveService
    {
        Task<string> CreateFolder(string name, string parentFolderId);

        Task<CreateStructureFolderResponse> CreateStructureFolder();
    }
}
