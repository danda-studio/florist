using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Users.Models.Response
{
    public class ProcessReferralModel
    {
        public int ReferalId { get; set; }
        public ProcessReferralItem ProcessReferralItem { get; set; } = new ProcessReferralItem();

    }
}
