using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Entities.Identity
{
    public class EmailSetting
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string FromEmail { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
    }
}
