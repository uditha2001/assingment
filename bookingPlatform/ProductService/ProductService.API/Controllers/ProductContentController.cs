using Microsoft.AspNetCore.Mvc;
using ProductService.API.DTO;
using ProductService.API.Services.serviceInterfaces;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/v1/product/content")]
    public class ProductContentController : ControllerBase
    {
        private readonly IProductContentService _productContentService;

        public ProductContentController(IProductContentService productContentService)
        {
            _productContentService = productContentService;
        }
        /// <summary>
        /// create a content, allow to create onecontent at a time
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateContent([FromQuery] long productId, [FromForm] List<IFormFile> images)
        {
            var result = await _productContentService.SaveProductImagesAsync(productId, images);
            if (!result)
                return StatusCode(500, "Failed to create content.");

            return Ok("Content created successfully.");
        }
        /// <summary>
        /// get all content for the given productId
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllContent([FromQuery] long productId)
        {
            var contentList = await _productContentService.GetAllContent(productId);
            return Ok(contentList);
        }


        /// <summary>
        /// delete a content using it id,only can delete content ,which created using internal system
        /// </summary>

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(long id)
        {
            var result = await _productContentService.DeleteContent(id);
            if (!result)
                return StatusCode(500, $"Failed to delete content with ID {id}.");
            return Ok($"Content with ID {id} deleted successfully.");
        }
    }
}
