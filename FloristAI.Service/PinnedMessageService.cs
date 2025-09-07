using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application
{
    public class PinnedMessageService : IPinnedMessageService
    {
        private readonly HashSet<long> _permanentMessages = new();

        public bool HasPermanentMessage(long chatId) => _permanentMessages.Contains(chatId);
        public void SetPermanentMessage(long chatId) => _permanentMessages.Add(chatId);
    }
}
