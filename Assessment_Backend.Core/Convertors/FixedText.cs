using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessment_Backend.Core.Convertors
{
    public class FixedText
    {
        public static string FexedEmail(string email)
        {
            return email.Trim().ToLower();
        }
    }
}
