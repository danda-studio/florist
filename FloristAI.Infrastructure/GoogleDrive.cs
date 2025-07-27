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
    }
}
