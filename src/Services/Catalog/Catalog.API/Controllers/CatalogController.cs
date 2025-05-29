using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

      

        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet]
        [Route("{id:length(24)}", Name = "GetProductById")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById([FromRoute] string id)
        {
            var product = await _repository.GetProduct(id);
            if (product == null)
            {
                _logger.LogError($"Product with id:{id} not found");
                return NotFound();
            }
            return Ok(product);
        }


        [HttpGet]
        [Route("category/{category}", Name = "GetProductByCategory")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory([FromRoute] string category)
        {
            var products = await _repository.GetProductByCategory(category);
            return Ok(products);
        }

        [HttpPost]
        [Route("", Name = "CreateProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                await _repository.CreateProduct(product);
                _logger.LogInformation($"Product with id:{product.Id} created successfully.");
                return CreatedAtRoute("GetProductById", new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating product: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:length(24)}", Name = "UpdateProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product product)
        {
            if (product.Id != id)
            {
                return BadRequest("Product ID mismatch");
            }

            var updatedProduct = await _repository.UpdateProduct(product);
            if (updatedProduct == null)
            {
                return NotFound();
            }
            return Ok(updatedProduct);
        }

        [HttpDelete]
        [Route("{id:length(24)}", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct([FromBody] string id)
        {
            var deletedProduct = await _repository.DeleteProduct(id);
            if (deletedProduct == null)
            {
                return NotFound();
            }
            return Ok(deletedProduct);
        }
    }
}
