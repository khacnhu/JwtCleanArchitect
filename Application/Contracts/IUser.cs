using Application.Dtos;
using Application.Dtos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IUser
    {
        Task<RegistrationResponse> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<LoginResponse> LoginUserAsync(LoginDto loginDto);    
    }
}
