using CdeMockService.DTO;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.DTO;
using System.Collections.Generic;

namespace CdeMockService.Controllers
{
    [ApiController]
    [Route("api/v1/product")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetMockData()
        {
            var products = new List<CdeDTO>();

            for (int i = 1; i <= 20; i++)
            {
                var product = new CdeDTO
                {
                    originId = i,
                    Name = $"Item {Convert.ToChar(64 + i)}",
                    Description = $"Description for Item {Convert.ToChar(64 + i)}",
                    Price = 50m + i * 10,
                    Currency = "USD",
                    ProductCategoryId = (i % 5) + 1,
                    owner = "owner " + i,
                    availableQuantity = 10 + i,
                    Attributes = new List<ProductAttributesDTO>(),
                    Contents = new List<ProductContentDTO>
                    {
                        new ProductContentDTO
                        {
                            contentId = 1000 + i,
                            Type = "Image",
                            Url = $"https://picsum.photos/seed/cde{i}/300/200",
                            Description = $"Image content for Item {Convert.ToChar(64 + i)}"
                        }
                    }
                };

                products.Add(product);
            }

            return Ok(products);
        }

        [HttpPost]
        public IActionResult PlaceOrder()
        {
            return Ok("Order placement successful");
        }
    }
}
