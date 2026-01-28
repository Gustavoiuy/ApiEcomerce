using Application.IRepository;
using Application.Dtos;
using Application.Responses;
using Domain.Entities;

using ApiEcommerce.Constants;

using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class ProductController : ControllerBase
    {

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(CacheProfileName = CacheProfiles.Default10)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productRepository.GetAllAsync();
            var productsDto = products.Adapt<List<ProductDto>>();
            return Ok(productsDto);
        }

        [AllowAnonymous]
        [HttpGet("{productId:int}", Name = "GetProduct")]
        [ResponseCache(CacheProfileName = CacheProfiles.Default10)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound($"El producto con el id {productId} no existe");
            }
            var productDto = product.Adapt<ProductDto>();
            return Ok(productDto);
        }

        [AllowAnonymous]
        [HttpGet("Paged", Name = "GetProductsInPage")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductsInPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Los parámetros de paginación no son válidos.");
            }
            var totalProducts = await _productRepository.GetTotalProductsAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            if (pageNumber > totalPages)
            {
                return NotFound("No hay más páginas disponibles por el momento intente mas tarde");
            }
            var products = await _productRepository.GetProductsInPagesAsync(pageNumber, pageSize);
            var productDto = products.Adapt<List<ProductDto>>();
            var paginationResponse = new ResponsesPagination<ProductDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Items = productDto
            };
            return Ok(paginationResponse);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            if (createProductDto == null)
            {
                return BadRequest(ModelState);
            }
            if (await _productRepository.ProductExistsAsync(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", "El producto ya existe");
                return BadRequest(ModelState);
            }
            if (!await _categoryRepository.CategoryExistsAsync(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"La categoría con el {createProductDto.CategoryId} no existe");
                return BadRequest(ModelState);
            }
            var product = createProductDto.Adapt<Product>();
            // Agregando imagen
            if (createProductDto.Image != null)
            {
                UploadProductImage(createProductDto, product);
            }
            else
            {
                product.ImgUrl = "https://placehold.co/300x300";
            }
            await _productRepository.AddAsync(product);
            var createdProduct = await _productRepository.GetByIdAsync(product.ProductId);
            var productoDto = createdProduct.Adapt<ProductDto>();
            return CreatedAtRoute("GetProduct", new { productId = product.ProductId }, productoDto);
        }

        [HttpGet("searchProductByCategory/{categoryId:int}", Name = "GetProductsForCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductsForCategory(int categoryId)
        {
            var products = await _productRepository.GetProductsForCategoryAsync(categoryId);
            if (products.Count == 0)
            {
                return NotFound($"Los productos con la categoría {categoryId} no existen");
            }
            var productsDto = products.Adapt<List<ProductDto>>();
            return Ok(productsDto);
        }

        [HttpGet("searchProductByNameDescription/{searchTerm}", Name = "SearchProducts")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchProducts(string searchTerm)
        {
            var products = await _productRepository.SearchProductsAsync(searchTerm);
            if (products.Count == 0)
            {
                return NotFound($"Los productos con el nombre o descripción '{searchTerm}' no existen");
            }
            var productsDto = products.Adapt<List<ProductDto>>();
            return Ok(productsDto);
        }

        [HttpPatch("buyProduct/{name}/{quantity:int}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> BuyProduct(string name, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
            {
                return BadRequest("El nombre del producto o la cantidad no son válidos");
            }
            var foundProduct = await _productRepository.ProductExistsAsync(name);
            if (!foundProduct)
            {
                return NotFound($"El producto con el nombre {name} no existe");
            }
            if (!await _productRepository.BuyProductAsync(name, quantity))
            {
                ModelState.AddModelError("CustomError", $"No se pudo comprar el producto {name} o la cantidad solicitada es mayor al stock disponible");
                return BadRequest(ModelState);
            }
            var units = quantity == 1 ? "unidad" : "unidades";
            return Ok($"Se compro {quantity} {units} del producto '{name}'");
        }



        [HttpPut("{productId:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduct(int productId, [FromForm] UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null)
            {
                return BadRequest(ModelState);
            }
            if (!await _productRepository.ProductExistsAsync(productId))
            {
                ModelState.AddModelError("CustomError", "El producto no existe");
                return BadRequest(ModelState);
            }
            if (!await _categoryRepository.CategoryExistsAsync(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"La categoría con el {updateProductDto.CategoryId} no existe");
                return BadRequest(ModelState);
            }
            var product = updateProductDto.Adapt<Product>();
            product.ProductId = productId;
            // Agregando imagen
            if (updateProductDto.Image != null)
            {
                UploadProductImage(updateProductDto, product);
            }
            else
            {
                product.ImgUrl = "https://placehold.co/300x300";
            }
            await _productRepository.UpdateAsync(product);
            return NoContent();
        }
        private void UploadProductImage(dynamic productDto, Product product)
        {
            string fileName = product.ProductId + Guid.NewGuid().ToString() + Path.GetExtension(productDto.Image.FileName);
            var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductsImages");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }
            var filePath = Path.Combine(imagesFolder, fileName);
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                file.Delete();
            }
            using var fileStream = new FileStream(filePath, FileMode.Create);
            productDto.Image.CopyTo(fileStream);
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            product.ImgUrl = $"{baseUrl}/ProductsImages/{fileName}";
            product.ImgUrlLocal = filePath;
        }


        [HttpDelete("{productId:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            if (productId == 0)
            {
                return BadRequest(ModelState);
            }
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound($"El producto con el id {productId} no existe");
            }
            await _productRepository.DeleteAsync(product);
            return NoContent();
        }

    }

}
