using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Entities
{
    public class CustomerWishList
    {
        public CustomerWishList(string id)
        {
            Id = id;
            Items = new List<WishListItem>();
        }
        public string Id { get; set; }
        public List<WishListItem> Items { get; set; }
    }
}
