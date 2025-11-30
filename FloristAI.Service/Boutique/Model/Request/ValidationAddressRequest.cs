using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Boutique.Model.Request
{
    public class ValidationAddressRequest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string LanguageCode { get; set; }
    }
}
