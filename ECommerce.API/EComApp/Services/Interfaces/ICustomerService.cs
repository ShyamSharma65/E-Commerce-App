using ECommerceAPI.DTOs;

namespace ECommerceAPI.Services.Interfaces
{
    public interface ICustomerService
    {
        public Task<ApiResponse<CustomerResponseDTO>> RegisterCustomer(CustomerRegistrationDTO registerCustomer);
        public Task<ApiResponse<LoginResponseDTO>> CustomerLogin(CustomerLoginDto customerLoginDto);
        public Task<ApiResponse<CustomerResponseDTO>> UpdateCustomerDetails(CustomerUpdateDTO customerUpdate);
        public Task<ApiResponse<ConfirmationResponse>> UpdatePassword(ChangePasswordDTO changePassword);
        public Task<ApiResponse<CustomerResponseDTO>> GetCustomerDetails(int customerId);

    }
}
