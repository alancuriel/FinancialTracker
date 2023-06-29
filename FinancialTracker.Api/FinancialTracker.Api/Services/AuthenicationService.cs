using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinancialTracker.Api.Dtos;
using FinancialTracker.Api.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FinancialTracker.Api;

public class AuthenicationService
{
    private readonly UserManager<User> userManager;
    private readonly IConfiguration config;

    public AuthenicationService(UserManager<User> userManager,
                                IConfiguration config)
    {
        this.userManager = userManager;
        this.config = config;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
    {
        try
        {
            User? existingUser = await userManager.FindByEmailAsync(registerRequest.Email);

            if (existingUser is not null)
            {
                return new RegisterResponse { Success = false, Message = "Email is already in use" };
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
                return new RegisterResponse
                {
                    Success = false,
                    Message = $"Create User failed {createdUserResult?.Errors?.First()?.Description}"
                };
            }

            IdentityResult addUserToRoleResult = await userManager
                .AddToRoleAsync(newUser, AuthorizationRoles.USER_ROLE);

            if (!addUserToRoleResult.Succeeded)
            {
                string? error = addUserToRoleResult?.Errors?.First()?.Description;
                return new RegisterResponse
                {
                    Success = false,
                    Message = $"Created User, but could not add user to role {error}"
                };
            }

            return new RegisterResponse
            {
                Success = true,
                Message = "User registerd successfuly!"
            };
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new RegisterResponse { Success = false, Message = ex.Message };
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                User? user = await userManager.FindByEmailAsync(request.Email);
                if (user is null || user.Email is null)
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
                DateTime expires = DateTime.Now.AddMinutes(30);

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
                    Email = user.Email,
                    Success = true,
                    UserId = user.Id.ToString()
                };
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.Message);
                return new LoginResponse { Success = false, Message = ex.Message };
            }
        }
}
