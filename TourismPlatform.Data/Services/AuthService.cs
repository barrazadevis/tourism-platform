using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TourismPlatform.Core.DTOs;
using TourismPlatform.Core.Entities;

namespace TourismPlatform.Data.Services
{
    public class AuthService : IAuthService
    {
        private readonly TourismDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(TourismDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string tenantSubdomain)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == tenantSubdomain && t.IsActive);

            if (tenant == null)
                throw new UnauthorizedAccessException("Tenant no encontrado");

            var user = await _context.Users
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.Email == request.Email && 
                                       u.TenantId == tenant.Id && 
                                       u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Credenciales inválidas");

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    TenantId = user.TenantId,
                    TenantName = user.Tenant.Name,
                    TenantSubdomain = user.Tenant.Subdomain
                }
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, string tenantSubdomain)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == tenantSubdomain && t.IsActive);

            if (tenant == null)
                throw new ArgumentException("Tenant no encontrado");

            var existingUser = await _context.Users
                .AnyAsync(u => u.Email == request.Email && u.TenantId == tenant.Id);

            if (existingUser)
                throw new ArgumentException("El usuario ya existe");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = "User",
                TenantId = tenant.Id,
                ApplicationId = tenant.ApplicationId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Load tenant for response
            await _context.Entry(user)
                .Reference(u => u.Tenant)
                .LoadAsync();

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    TenantId = user.TenantId,
                    TenantName = user.Tenant.Name,
                    TenantSubdomain = user.Tenant.Subdomain
                }
            };
        }
        public async Task<User?> GetUserFromTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    return await _context.Users
                        .Include(u => u.Tenant)
                        .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }
        
        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

            var claims = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("name", user.FullName),
                new Claim("email", user.Email),
                new Claim("role", user.Role),
                new Claim("tenant_id", user.TenantId.ToString()),
                new Claim("tenant_subdomain", user.Tenant.Subdomain)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["JwtSettings:ExpirationHours"])),
                Issuer = _configuration["JwtSettings:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}