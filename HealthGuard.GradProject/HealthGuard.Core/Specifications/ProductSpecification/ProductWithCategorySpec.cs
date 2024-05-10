using HealthGuard.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Core.Specifications.ProductSpecification
{
    public class ProductWithCategorySpec : BaseSpecifications<Product>
    {

        public ProductWithCategorySpec(ProductSpecParams productSpec) : base( 
            p =>
                  (string.IsNullOrEmpty(productSpec.Search) || p.Name.ToLower().Contains(productSpec.Search.ToLower())) &&
                  (!productSpec.CategoryId.HasValue || p.CategoryId == productSpec.CategoryId.Value)
        )
        {
            Includes.Add(p => p.Category);
            if (!string.IsNullOrEmpty(productSpec.Sort))
            {
                switch (productSpec.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDes(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name);
                        break;

                }
            }
            else
            {
                AddOrderBy(p => p.Name);
            }
           ApplyPagination((productSpec.PageIndex - 1) * productSpec.PageSize, productSpec.PageSize);
        }
        public ProductWithCategorySpec(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.Category);
        }
    }
}
