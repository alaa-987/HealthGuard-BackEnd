using HealthGuard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Specifications.ProductSpecification
{
    public class ProductWithFiltrationForCount: BaseSpecifications<Product>
    {
        public ProductWithFiltrationForCount(ProductSpecParams productSpec) :
            base(p =>
                  (string.IsNullOrEmpty(productSpec.Search) || p.Name.ToLower().Contains(productSpec.Search.ToLower())) &&
                  (!productSpec.CategoryId.HasValue || p.CategoryId == productSpec.CategoryId.Value))
        {

        }
    }
}
