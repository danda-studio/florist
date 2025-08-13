using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Infrastructure.Models.Response
{
    public class FindFolderByNameResponse
    {
        public FindFolderByNameResponse(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
