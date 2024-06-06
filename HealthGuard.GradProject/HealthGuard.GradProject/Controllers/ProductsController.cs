using AutoMapper;
using HealthGuard.Core.Entities;
using HealthGuard.Core.Repository.contract;
using HealthGuard.Core.Specifications.ProductSpecification;
using HealthGuard.GradProject.DTO;
using HealthGuard.GradProject.Errors;
using HealthGuard.GradProject.Helpers;
using HealthGurad.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthGuard.GradProject.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<ProductCategory> _categoryRepo;
        private readonly IMapper _mapper;
        private readonly StoreContext _dbContext;

        public ProductsController(IGenericRepository<Product> productRepo, IGenericRepository<ProductCategory> categoryRepo, IMapper mapper,StoreContext dbContext)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _mapper = mapper;
            _dbContext = dbContext;
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
            return Ok(new Pagination<ProductToReturnDto>(count, data));
        }
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]

        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var spec = new ProductSpecParamId(id);
            var product = await _productRepo.GetWithSpecAsync(spec);
            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }
            return Ok(_mapper.Map<Product, ProductToReturnDto>(product));
        }
        [HttpGet("byCategory/{categoryId}/products")]
        public async Task<ActionResult<IReadOnlyCollection<ProductToReturnDto>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var spec = new ProductWithCategorySpec(categoryId);
                var products = await _productRepo.GetAllWithSpecAsync(spec);

                if (products == null || products.Count == 0)
                {
                    return NotFound(new ApiResponse(404, "No products found in the specified category."));
                }

                var productDtos = _mapper.Map<IReadOnlyCollection<ProductToReturnDto>>(products);

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching products by category: " + ex.Message);
                throw;
            }
        }
        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyCollection<ProductCategoryDto>>> GetCategory()
        {
            try
            {
                var categories = await _categoryRepo.GetAllAsync();
                var categoryDtos = _mapper.Map<IReadOnlyCollection<ProductCategoryDto>>(categories);
                return Ok(categoryDtos);

            }
            catch (Exception ex)
            {

                Console.WriteLine("Error fetching categories: " + ex.Message);
                throw;
            }
        }
        [HttpPost("add-product")]
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductToReturnDto>> AddProduct(ProductToReturnDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse(400, "Invalid product data."));
            }

            var product = _mapper.Map<ProductToReturnDto, Product>(productDto);

            try
            {
                await _productRepo.Add(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
                return StatusCode(500, new ApiResponse(500, "Failed to add the product."));
            }

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, _mapper.Map<ProductToReturnDto>(product));
        }
        [HttpPost]
        public async Task<ActionResult<ProductToReturnDto>> CreateProduct([FromBody] ProductCreateDto productCreateDto)
        {
            var product = _mapper.Map<ProductCreateDto, Product>(productCreateDto);
            await _productRepo.Add(product);
            await _dbContext.SaveChangesAsync();
            var productToReturn = _mapper.Map<Product, ProductToReturnDto>(product);
            return CreatedAtAction(nameof(GetProduct), new { id = productToReturn.Id }, productToReturn);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> UpdateProduct(int id, ProductToReturnDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse(400, "Invalid product data."));
            }

            var product = await _productRepo.GetAsync(id);
            if (product == null)
            {
                return NotFound(new ApiResponse(404, "Product not found."));
            }

            _mapper.Map(productDto, product);

            try
            {
                _productRepo.Update(product);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error updating product: {ex.Message}");
                return StatusCode(500, new ApiResponse(500, "Failed to update the product."));
            }

            return Ok(_mapper.Map<ProductToReturnDto>(product));
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _productRepo.GetAsync(id);
            if (product == null)
            {
                return NotFound(new ApiResponse(404, "Product not found."));
            }

            try
            {
                _productRepo.Delete(product);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product: {ex.Message}");
                return StatusCode(500, new ApiResponse(500, "Failed to delete the product."));
            }

            return Ok(new ApiResponse(200, "Product deleted successfully."));
        }
    }
}
