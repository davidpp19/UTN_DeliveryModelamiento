using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;
using Delivery.Servicios.Interfaces;

namespace Delivery.Servicios.Implementaciones
{
    public class SeguridadService : ISeguridadService
    {
        private readonly IConfiguration _configuration;

        public SeguridadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string HashearPassword(string password)
        {
            return BC.HashPassword(password);
        }

        public bool VerificarPassword(string password, string hash)
        {
            return BC.Verify(password, hash);
        }

        public string GenerarTokenJwt(long usuarioId, string email, string rol)
        {
            // Nota: En un entorno real, la clave secreta y la configuración provienen de appsettings.json
            // Aquí se asume que hay un "Jwt:Key" en la configuración.
            var keyConfig = _configuration["Jwt:Key"] ?? "ClaveSuperSecretaParaDesarrolloQueDeberiaEstarEnAppsettings.123456789";
            var issuer = _configuration["Jwt:Issuer"] ?? "DeliveryAPI";
            var audience = _configuration["Jwt:Audience"] ?? "DeliveryClient";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyConfig));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
