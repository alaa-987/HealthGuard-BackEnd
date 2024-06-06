using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Entities.Identity
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string PatientName { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string AppNurseId { get; set; }  
        public AppNurse AppNurse { get; set; }
        [Column("AppUserId")]
        public string AppUserId { get; set; } 
        public AppUser User { get; set; }
    }
}
