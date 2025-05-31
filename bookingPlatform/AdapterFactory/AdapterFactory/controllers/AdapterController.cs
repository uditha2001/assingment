using AdapterFactory.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.DTO;
using ProductService.API.DTO;

namespace AdapterFactory.controllers
{
    [ApiController]
    [Route("api/v1/Adapter")]
    public class AdapterController : Controller
    {
        private readonly IAdapterFactory _adapterFactory;
        public AdapterController(IAdapterFactory adapterFactory)
        {
            _adapterFactory = adapterFactory;
        }
        [HttpGet]
        public async Task<IActionResult> getAllContents()

        {
            try
            {
                var products = await _adapterFactory.getAllProducts();
                return Ok(products);
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching product contents.", error = ex.Message });
            }
        }
        [HttpPost]
        public async  Task<IActionResult> placeOrder([FromBody] CheckoutDTO order)
        {
            try
            {
                bool res = await _adapterFactory.placeOrder(order);
                return Ok(res);

            }
            catch (Exception e)
            {
                return  StatusCode(500, new { message = "An error occurred while fetching product contents.", error = e.Message });

            }
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> checkout([FromBody] CheckoutDTO orderDetails)
        {
            try
            {
                bool res = await _adapterFactory.checkoutOrder(orderDetails);
                return Ok(res);

            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "An error occurred while fetching product contents.", error = e.Message });

            }
        }


    }
}
