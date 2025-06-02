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
        public async Task<IActionResult> GetAllContents()

        {
            try
            {
                var products = await _adapterFactory.GetAllProductsAsync();
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
        public async  Task<IActionResult> PlaceOrder([FromBody] CheckoutDTO order)
        {
            try
            {
                bool res = await _adapterFactory.PlaceOrder(order);
                return Ok(res);

            }
            catch (Exception e)
            {
                return  StatusCode(500, new { message = "An error occurred while fetching product contents.", error = e.Message });

            }
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDTO orderDetails)
        {
            try
            {
                if (orderDetails == null)
                    return BadRequest("Invalid checkout data.");

                bool res = await _adapterFactory.CheckoutOrder(orderDetails);
                return Ok(res);

            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "An error occurred while fetching product contents.", error = e.Message });

            }
        }


    }
}
