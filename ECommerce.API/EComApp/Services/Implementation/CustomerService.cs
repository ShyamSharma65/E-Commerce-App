using Azure;
using ECommerceAPI.AppDbContext;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ECommerceAPI.Services.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly EComDbContext _eComDbContext;
        public CustomerService(EComDbContext eComDbContext)
        {
            _eComDbContext = eComDbContext;
        }
        public async Task<ApiResponse<LoginResponseDTO>> CustomerLogin(CustomerLoginDto customerLoginDto)
        {
            try
            {
                var user = await _eComDbContext.Customers.FirstOrDefaultAsync(c => c.Email == customerLoginDto.Email);
                if(user == null || user.Password != customerLoginDto.Password)
                {
                    return new ApiResponse<LoginResponseDTO>(401, "Invalid email or password");
                }
                var response = new LoginResponseDTO
                {
                    CustomerId = user.Id,
                    CustomerName = user.FirstName + " " + user.MiddleName + " " + user.LastName,
                    Message = "Login Successfull"
                };

                return new ApiResponse<LoginResponseDTO>(200, response);

            }
            catch(Exception e)
            {
                return new ApiResponse<LoginResponseDTO>(500, "Internal Server Error");
            }
        }


        public async Task<ApiResponse<CustomerResponseDTO>> RegisterCustomer(CustomerRegistrationDTO registerCustomer)
        {
            try
            {
                var user = await _eComDbContext.Customers.FirstOrDefaultAsync(c => c.Email == registerCustomer.Email);
                if(user != null)
                {
                    return new ApiResponse<CustomerResponseDTO>(409, "User already registered with this Email");
                }
                user = new Customer
                {
                    FirstName = registerCustomer.FirstName,
                    MiddleName = registerCustomer.MiddleName,
                    LastName = registerCustomer.LastName,
                    PhoneNumber = registerCustomer.Phone,
                    Password = registerCustomer.Password,
                    Email = registerCustomer.Email,
                    DateOfBirth = registerCustomer.DateOfBirth,
                    IsActive = true
                };
                await _eComDbContext.Customers.AddAsync(user);
                await _eComDbContext.SaveChangesAsync();
                CustomerResponseDTO crd = new CustomerResponseDTO()
                {
                    FirstName = registerCustomer.FirstName,
                    MiddleName = registerCustomer.MiddleName,
                    LastName = registerCustomer.LastName,
                    Phone = registerCustomer.Phone,
                    Email = registerCustomer.Email,
                    DateOfBirth = registerCustomer.DateOfBirth
                };
                return new ApiResponse<CustomerResponseDTO>(201, crd);
            }
            catch (Exception e)
            {
                return new ApiResponse<CustomerResponseDTO>(500, "Internal Server Error");
            }
        }

        public async Task<ApiResponse<CustomerResponseDTO>> UpdateCustomerDetails(CustomerUpdateDTO customerUpdate)
        {
            try
            {
                var user = await _eComDbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerUpdate.CustomerId);
                if(user == null)
                {
                    return new ApiResponse<CustomerResponseDTO>(404, "User not found");
                }
                user.FirstName = customerUpdate.FirstName;
                user.MiddleName = customerUpdate.MiddleName;
                user.LastName = customerUpdate.LastName;
                user.PhoneNumber = customerUpdate.Phone;
                user.Email = customerUpdate.Email;
                user.DateOfBirth = customerUpdate.DateOfBirth;

                await _eComDbContext.SaveChangesAsync();

                var response = new CustomerResponseDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.PhoneNumber

                };

                return new ApiResponse<CustomerResponseDTO>(200,response);

            }
            catch(Exception e)
            {
                return new ApiResponse<CustomerResponseDTO>(500, "Internal Server Error");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> UpdatePassword(ChangePasswordDTO changePassword)
        {
            try
            {
                var user = await _eComDbContext.Customers.FirstOrDefaultAsync(c => c.Id == changePassword.CustomerId && c.Password == changePassword.CurrentPassword);
                if (user == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "User not found");
                }
                user.Password = changePassword.NewPassword;
                await _eComDbContext.SaveChangesAsync();
                var res = new ConfirmationResponse
                {
                    Message = "Password updated sucessfully"
                };
                return new ApiResponse<ConfirmationResponse>(204, res);
            }
            catch(Exception e)
            {
                return new ApiResponse<ConfirmationResponse>(500, "Internal Server Error");
            }
        }
        public async Task<ApiResponse<CustomerResponseDTO>> GetCustomerDetails(int customerId)
        {
            try
            {
                var user = await _eComDbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
                if (user == null)
                {
                    return new ApiResponse<CustomerResponseDTO>(404, "User not found");
                }
                var response = new CustomerResponseDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth
                };
                return new ApiResponse<CustomerResponseDTO>(200,response);
            }
            catch(Exception e)
            {
                return new ApiResponse<CustomerResponseDTO>(500, "Internal Server Error");
            }
            
        }
    }
}
