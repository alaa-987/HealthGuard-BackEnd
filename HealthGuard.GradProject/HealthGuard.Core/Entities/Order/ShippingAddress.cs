using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Entities.Order
{
    public class ShippingAddress
    {
        public ShippingAddress()
        {

        }
        public ShippingAddress(string fname, string lname, string street, string city, string country)
        {
            FName = fname;
            LName = lname;
            Street = street;
            City = city;
            Country = country;
        }

        public string FName { get; set; }
        public string LName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
