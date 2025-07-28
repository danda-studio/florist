using FloristAI.Application.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleDrive
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly IGoogleDrive _googleDrive;

        public GoogleDriveService(IGoogleDrive googleDrive)
        {
            _googleDrive = googleDrive;
        }

        public async Task<string> CreateFolder(string name, string parentFolderId)
        {
            return await _googleDrive.CreateFolder(name, parentFolderId);
        }
    }
}
