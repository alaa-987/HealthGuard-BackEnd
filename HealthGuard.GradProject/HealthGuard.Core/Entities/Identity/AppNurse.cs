using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Entities.Identity
{
    public class AppNurse : AppUser
    {
        public string NurseName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string PicUrl { get; set; }
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public string Hospital { get; set; }
        public string Specialty { get; set; }
    }
}
