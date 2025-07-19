using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Users.ReferralCode
{
    public interface IReferralService
    {
        string GetReferralLink(int Id);
        byte[] GetReferralQrCode(int Id);
    }
}
