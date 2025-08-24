using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.Models
{
    public class MessageContext
    {
        public string Message { get; set; } = string.Empty;
        public long ChatId { get; set; }
        public string? Parameter { get; set; } = string.Empty;

        public string? Username { get; set; } = string.Empty;

        public PartnerPayload PartnerPayload { get; set; } = new PartnerPayload();

    }
}
