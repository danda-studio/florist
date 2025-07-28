using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Store
{
    public interface IGoogleDrive
    {
        Task<string> CreateFolder(string name, string parentFolderId);
    }
}
