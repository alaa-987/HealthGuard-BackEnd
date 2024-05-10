using HealthGuard.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthGurad.Repository.Data
{
    public static class StoreContextSeed
    {
        public async static Task SeedAsync(StoreContext _dbContext)
        {
            if (_dbContext.DeliveryMethod.Count() == 0)
            {
                var DeliveryMethodsData = File.ReadAllText("../HealthGurad.Repository/Data/DataSeeding/delivery.json");
                var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);
                if (DeliveryMethods?.Count() > 0)
                {
                    foreach (var delivery in DeliveryMethods)
                    {
                        _dbContext.Set<DeliveryMethod>().Add(delivery);
                    }


                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
