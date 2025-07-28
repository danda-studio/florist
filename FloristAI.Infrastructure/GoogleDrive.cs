using FloristAI.Application.Store;
using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Infrastructure
{
    public class GoogleDrive : IGoogleDrive
    {
        private readonly DriveService _driveService;

        public GoogleDrive(DriveService driveService)
        {
            _driveService = driveService;
        }

        public async Task<string> CreateFolder(string name, string? parentId = null)
        {
            // 1. Ищем папку по имени
            var request = _driveService.Files.List();
            request.Q = $"name='{name}' and mimeType='application/vnd.google-apps.folder'" +
                        (parentId != null ? $" and '{parentId}' in parents" : "");
            var result = await request.ExecuteAsync();

            if (result.Files.Count > 0)
                return result.Files[0].Id;

            // 2. Создаем папку
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name,
                MimeType = "application/vnd.google-apps.folder",
                Parents = parentId != null ? new List<string> { parentId } : null
            };

            var createRequest = _driveService.Files.Create(fileMetadata);
            createRequest.Fields = "id";
            var folder = await createRequest.ExecuteAsync();

            return folder.Id;
        }
    }
}
