using Microsoft.AspNetCore.Mvc;
using ProductService.API.DTO;
using ProductService.API.Services.serviceInterfaces;
using System.Linq.Expressions;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/v1/product/attribute")]
    public class ProductAttributeController : Controller
    {
        private readonly IProductAttributeService _productAttributeService;
        private readonly ILogger<ProductAttributeController> _logger;

        public ProductAttributeController(IProductAttributeService productAttributeService,ILogger<ProductAttributeController> logger)
        {
            _productAttributeService = productAttributeService;
            _logger = logger;

        }

        /// <summary>
        /// return all Attributes assign to a productId
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> getAllAttributes([FromQuery] long productId)
        {
            try
            {
                List<ProductAttributesDTO> productAttributesDTOs = await _productAttributeService.getAllAttributes(productId);
                return Ok(productAttributesDTOs);
            }
            catch (Exception e)
            {

                Console.WriteLine($"Exception: {e.GetType().Name}, Message: {e.Message}, StackTrace: {e.StackTrace}");
                _logger.LogError(e, "An error occurred while processing the request.");
                return BadRequest(new List<ProductAttributesDTO> { new ProductAttributesDTO() });
            }

            
        }

        /// <summary>
        /// delete a attribute using attriuteId
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> deleteAttribute([FromQuery] long attributeId)
        {
            bool result =await _productAttributeService.deleteAttribute(attributeId);
            if (result)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        /// <summary>
        /// update atrribute of a product
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> updateAttribute([FromBody] List<ProductAttributesDTO> attributes, [FromQuery]long productId)
        {
            {
                bool result = await _productAttributeService.updateAttribute(attributes,productId);
                if (result)
                {
                    return Ok();

                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");

                }
            }

        }

        /// <summary>
        /// create multiple productAttributes
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> createAtrribute([FromBody]List<ProductAttributesDTO> productsAtribute, [FromQuery] long productId)
        {
            bool result = await _productAttributeService.createAttribute(productsAtribute, productId);
            if (result)
            {
                return Ok();

            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");

            }
        }
    }
}
