using FloristAI.Application.GoogleDrive.Models.Response;
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

        public async Task<CreateStructureFolderResponse> CreateStructureFolder()
        {
            string reportRootId = "1_Mbpr5Y9wVK3AnLdSnpL9Eg4WhV9xdGD";
            string currentYear = DateTime.Now.Year.ToString("yyyy");

            // 1. Проверяем и создаем папку года
            var yearFolder = await FindOrCreateFolder(currentYear, reportRootId);

            // 2. Проверяем и создаем папку "Партнеры"
            var partnersFolder = await FindOrCreateFolder("Партнеры", yearFolder.FileId);

            // 3. Проверяем и создаем остальные папки
            var privateFolder = await FindOrCreateFolder("Приватная часть", partnersFolder.FileId);
            var publicFolder = await FindOrCreateFolder("Публичная часть", partnersFolder.FileId);

            return new CreateStructureFolderResponse
            {
                PrivateFolderId = privateFolder.FileId,
                PublicFolderId = publicFolder.FileId,
            };
        }

        public async Task<(string FileId, string FileName)> FindOrCreateFolder(string folderName, string parentFolderId)
        {
            // Пытаемся найти существующую папку
            var existingFolder = await _googleDrive.FindFolderByName(folderName, parentFolderId);

            if (existingFolder.HasValue)
            {
                return (existingFolder.Value.Id, existingFolder.Value.Name);
            }

            // Если папка не найдена - создаем новую
            var newFolderId = await _googleDrive.CreateFolder(folderName, parentFolderId);
            return (newFolderId, folderName);
        }
    }
}
