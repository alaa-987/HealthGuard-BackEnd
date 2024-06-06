using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Entities
{
    public class WishListItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string PicUrl { get; set; }
        public decimal Price { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public int Quanntity { get; set; }
    }
}
