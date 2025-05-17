using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDomainLayer.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace InventoryAppServicesLayer.AuthorizationFilter
{
    public class JwtAuthorizationFilter : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public JwtAuthorizationFilter(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }


        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpcontext = _httpContextAccessor.HttpContext;
            // Retrieve the Authorization header from the request
            var authHeader = httpcontext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader))
            {
                // If Authorization header is missing, return 400 Bad Request
                throw new UserUnauthenticatedException("Invalid request: Missing Authorization header.");
            }

            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new UserUnauthenticatedException("Unauthorized: Invalid Authorization header format. Expected 'Bearer <token>'.");
            }

            // Extract the token by removing "Bearer "
            var token = authHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                throw new UserUnauthenticatedException("Unauthorized: Token is missing.");
            }

            // Validate the token
            if (!await ValidateTokenAsync(token))
            {

                throw new UserUnauthenticatedException("Forbidden: Invalid or expired token.");
            }
        }

        private async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero, // Allow the token to expire immediately if it's expired
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Token validation failed: {ex.Message}");
                return false;
            }
        }
    }
}
