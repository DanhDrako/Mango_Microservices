using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Extensions;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.RequestHelpers;
using Mango.Services.ProductAPI.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor; // Add IHttpContextAccessor for accessing HttpContext

        public ProductService(IMapper mapper, AppDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _db = db;
            _httpContextAccessor = httpContextAccessor; // Initialize IHttpContextAccessor
        }

        /// <summary>
        /// Create or update a product in the database.
        /// </summary>
        /// <param name="productDto"></param>
        /// <returns>ProductDto</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            if (productDto == null)
            {
                throw new ArgumentNullException(nameof(productDto), "Product cannot be null");
            }

            // 1. Mapper product
            var product = _mapper.Map<Product>(productDto);

            // 2. Added product if not exist
            if (productDto.ProductId <= 0)
            {
                _db.Products.Add(product);
                _db.SaveChanges();
            }

            // 3. Save image product
            if (productDto.File != null)
            {
                string fileName = product.ProductId + Path.GetExtension(productDto.File.FileName);
                string filePath = @"wwwroot\ProductImages\" + fileName;

                string path = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                FileInfo file = new FileInfo(path);
                if (file.Exists)
                {
                    file.Delete();
                }
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await productDto.File.CopyToAsync(stream);
                }

                // Use IHttpContextAccessor to access HttpContext
                var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");

                var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}{httpContext.Request.PathBase.Value}";

                product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                product.ImageLocalPath = filePath;
            }
            else if (productDto.ImageUrl == null)
            {
                product.ImageUrl = "https://placehold.co/600x400";
            }

            // 4. Update to database
            _db.Products.Update(product);
            _db.SaveChanges();

            var res = _mapper.Map<ProductDto>(product);
            return res;
        }

        public async Task<int> DeleteProduct(int id)
        {
            var product = await _db.Products.FindAsync(id) ?? throw new KeyNotFoundException($"Product with ID {id} not found.");
            if (!string.IsNullOrEmpty(product.ImageLocalPath))
            {
                var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                FileInfo file = new FileInfo(oldFilePathDirectory);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            _db.Products.Remove(product);
            return await _db.SaveChangesAsync();
        }

        public async Task<Filter> GetFilters()
        {
            var types = await _db.Products.Select(p => p.Type).Distinct().ToListAsync();
            var brands = await _db.Products.Select(p => p.Brand).Distinct().ToListAsync();
            var filter = new Filter
            {
                Types = types,
                Brands = brands
            };
            return filter;
        }

        public async Task<ProductDto> GetProductById(int id)
        {
            var product = await _db.Products.FindAsync(id) ?? throw new KeyNotFoundException($"Product with ID {id} not found.");
            var res = _mapper.Map<ProductDto>(product);
            return res;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts(ProductParams productParams)
        {
            var query = _db.Products
            .Sort(productParams.OrderBy)
            .Search(productParams.SearchTerm)
            .Filter(productParams.Brands, productParams.Types)
            .AsQueryable();

            // Fetch paginated list of products
            var products = await PagedList<Product>.ToPagedList(query, productParams.PageNumber, productParams.PageSize);

            // Map Product entities to ProductDto
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            // Use IHttpContextAccessor to add pagination headers
            var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");
            httpContext.Response.AddPaginationHeaders(products.Metadata);

            return productDtos;
        }
    }
}
