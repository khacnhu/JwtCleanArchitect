using Application.Contracts;
using Application.Dtos;
using Application.Dtos.Response;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Infrastructure.Repository
{
    internal class UserRepository : IUser
    {
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;

        public UserRepository(AppDbContext appDbContext, IConfiguration configuration) 
        {
            this.context = appDbContext;
            this.configuration = configuration;
        }


        public async Task<LoginResponse> LoginUserAsync(LoginDto loginDto)
        {
            var user = await FindUserByEmail(loginDto.Email);
            if (user == null) 
            {
                return new LoginResponse(false, "User not found, sorry");
            }
            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password );
            if (checkPassword) 
            {
                return new LoginResponse(true, "Login Successfully", GenerateJwtToken(user, configuration));
            } else
            {
                return new LoginResponse(false, "Invalid credentials - Password is not correct");
            }

        }

        private async Task<ApplicationUser?> FindUserByEmail(string email) 
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }


        private static string GenerateJwtToken(ApplicationUser user, IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Email, user.Email!),

            };

            var token = new JwtSecurityToken(
                    issuer: configuration["Jwt:Issuer"],  
                    audience: configuration["Jwt:Audience"],
                    claims: userClaims,
                    expires: DateTime.Now.AddDays(5),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RegistrationResponse> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            var user = await FindUserByEmail(registerUserDto.Email);
            if (user != null)
            {
                return new RegistrationResponse(false, "User already existed");
            }


            context.Users.Add(new ApplicationUser()
            {
                Name = registerUserDto.Name,
                Email = registerUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password)
            });


            await context.SaveChangesAsync();
            return new RegistrationResponse(true, "Registration Successfully");

        }
    }
}
