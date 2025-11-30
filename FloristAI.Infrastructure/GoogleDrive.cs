using FloristAI.Application.Store;
using Google.Apis.Drive.v3;


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
                        (parentId != null ? $" and '{parentId}' in parents and trashed = false" : "");
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

            // 3. Назначаем права доступа
            if (name == "Приватная часть")
            {
                // Удаляем публичные разрешения (anyone и domain)
                var permissions = await _driveService.Permissions.List(folder.Id).ExecuteAsync();
                foreach (var permissionPrivate in permissions.Permissions)
                {
                    if (permissionPrivate.Type == "anyone" || permissionPrivate.Type == "domain")
                    {
                        await _driveService.Permissions.Delete(folder.Id, permissionPrivate.Id).ExecuteAsync();
                    }
                }

                // Добавляем доступ конкретному пользователю
                var permission = new Google.Apis.Drive.v3.Data.Permission()
                {
                    Type = "user",
                    Role = "reader",       
                    EmailAddress = "downh068@gmail.com"
                };

                var permissionRequest = _driveService.Permissions.Create(permission, folder.Id);
                permissionRequest.Fields = "id";
                await permissionRequest.ExecuteAsync();
            }
            else
            {
                // Папка публичная — расшариваем для всех
                var permissionPublic = new Google.Apis.Drive.v3.Data.Permission()
                {
                    Type = "anyone",
                    Role = "reader"
                };

                var permissionRequest = _driveService.Permissions.Create(permissionPublic, folder.Id);
                permissionRequest.Fields = "id";
                await permissionRequest.ExecuteAsync();
            }

            return folder.Id;
        }



        public async Task<(string Id, string Name)?> FindFolderByName(string name, string parentFolderId)
        {
            try
            {
                var request = _driveService.Files.List();
                request.Q = $"name = '{name}' and '{parentFolderId}' in parents and mimeType = 'application/vnd.google-apps.folder' and trashed = false";
                request.Fields = "files(id, name)";
                request.SupportsAllDrives = true;
                request.IncludeItemsFromAllDrives = true;

                var result = await request.ExecuteAsync();
                var file = result.Files.FirstOrDefault();

                return file != null ? (file.Id, file.Name) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при поиске папки: {ex.Message}");
                return null;
            }
        }
    }
}
