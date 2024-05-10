using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Entities
{
    public class Product: BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? About { get; set; }
        public string PictureUrl { get; set; }
        public Decimal Price { get; set; }
        [Column("NewRate")]
        public Decimal Rate { get; set; }
        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; }
    }
}
