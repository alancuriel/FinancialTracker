using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinancialTracker.Api.Dtos;
using FinancialTracker.Api.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FinancialTracker.Api.Services;

public class AuthenicationService : IAuthenicationService
{
    private readonly UserManager<User> userManager;
    private readonly IConfiguration config;

    public AuthenicationService(UserManager<User> userManager, IConfiguration config)
    {
        this.userManager = userManager;
        this.config = config;
    }

    public async Task<GenericResponse> RegisterAsync(RegisterRequest registerRequest)
    {
        try
        {
            User? existingUser = await userManager.FindByEmailAsync(registerRequest.Email);

            if (existingUser is not null)
            {
                return new GenericResponse { Success = false, Message = "Email is already in use" };
            }

            User newUser = new()
            {
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Email = registerRequest.Email,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                UserName = registerRequest.Email
            };

            IdentityResult createdUserResult = await userManager
                .CreateAsync(newUser, registerRequest.Password);

            if (!createdUserResult.Succeeded)
            {
                string? error = createdUserResult?.Errors?.FirstOrDefault()?.Description ?? "";
                return new GenericResponse
                {
                    Success = false,
                    Message = $"Create User failed {error}"
                };
            }

            IdentityResult addUserToRoleResult = await userManager
                .AddToRoleAsync(newUser, AuthorizationRoles.USER_ROLE);

            if (!addUserToRoleResult.Succeeded)
            {
                string? error = addUserToRoleResult?.Errors?.FirstOrDefault()?.Description ?? "";
                return new GenericResponse
                {
                    Success = false,
                    Message = $"Created User, but could not add user to role {error}"
                };
            }

            return new GenericResponse
            {
                Success = true,
                Message = "User registered successfuly!"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new GenericResponse { Success = false, Message = ex.Message };
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            User? user = await userManager.FindByEmailAsync(request.Email);
            if (user is null || user.Email is null || !await CheckPassword(request, user))
            {
                return new LoginResponse { Message = "Invalid email/password", Success = false };
            }

            List<Claim> claims = new()
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

            IList<string> roles = await userManager
                .GetRolesAsync(user);


            IEnumerable<Claim> roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));

            claims.AddRange(roleClaims);

            string keyStr = config["JWT_SECRET_KEY"]!;

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(keyStr));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
            int expireMinutes = 30;
            DateTime expires = DateTime.Now.AddMinutes(expireMinutes);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:5001",
                audience: "https://localhost:5001",
                claims,
                signingCredentials: creds,
                expires: expires
            );

            return new LoginResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                Message = "Login Successful",
                ExpiresIn = expireMinutes*60,
                TokenType = "Bearer",
                Success = true
            };
        }
        catch (System.Exception ex)
        {
            Console.Write(ex.Message);
            return new LoginResponse { Success = false, Message = ex.Message };
        }
    }

    private async Task<bool> CheckPassword(LoginRequest request, User user)
    {
        return await userManager.CheckPasswordAsync(user, request.Password);
    }

    public async Task<User?> GetCurrentUserAsync(HttpContext httpContext)
    {
        return await userManager.GetUserAsync(httpContext.User);
    }
}
