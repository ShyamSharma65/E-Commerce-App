using ECommerceAPI.DTOs;
using ECommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        
        [HttpPost("registercustomer")]
        public async Task<ActionResult<ApiResponse<CustomerResponseDTO>>> RegisterCustomer([FromBody] CustomerRegistrationDTO customerRegistration)
        {
            var response = await _customerService.RegisterCustomer(customerRegistration);
            if(response.StatusCode != 201)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPut("updatecustomer")]
        public async Task<ActionResult<ApiResponse<CustomerResponseDTO>>> UpdateCustomerDetails([FromBody] CustomerUpdateDTO customerUpdate)
        {
            var response = await _customerService.UpdateCustomerDetails(customerUpdate);
            return StatusCode(response.StatusCode, response);
        }


    }
}
