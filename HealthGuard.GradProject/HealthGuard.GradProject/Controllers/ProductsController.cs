using AutoMapper;
using HealthGuard.Core.Entities;
using HealthGuard.Core.Repository.contract;
using HealthGuard.Core.Specifications.ProductSpecification;
using HealthGuard.GradProject.DTO;
using HealthGuard.GradProject.Errors;
using HealthGuard.GradProject.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthGuard.GradProject.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<ProductCategory> _categoryRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productRepo, IGenericRepository<ProductCategory> categoryRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] ProductSpecParams ProductSepc)
        {
            var spec = new ProductWithCategorySpec(ProductSepc);
            var products = await _productRepo.GetAllWithSpecAsync(spec);
            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);
            var countSpec = new ProductWithCategorySpec(ProductSepc);
            var count = await _productRepo.GetCountAsync(countSpec);
            //JsonResult result = new JsonResult(products);
            return Ok(new Pagination<ProductToReturnDto>(ProductSepc.PageIndex, ProductSepc.PageSize, count, data));
        }
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]

        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var spec = new ProductWithCategorySpec(id);
            var product = await _productRepo.GetWithSpecAsync(spec);
            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }
            return Ok(_mapper.Map<Product, ProductToReturnDto>(product));
        }
        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyCollection<ProductCategory>>> GetCategory()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return Ok(categories);
        }
        
    }
}
