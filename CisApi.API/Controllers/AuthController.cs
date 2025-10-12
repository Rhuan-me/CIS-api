// CisApi.API/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CisApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var client = _httpClientFactory.CreateClient();
            var javaApiUrl = _configuration["JavaApi:Url"];

            // --- INÍCIO DA ALTERAÇÃO ---
            // Configura o serializador para usar camelCase
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(loginDto, serializerOptions),
                Encoding.UTF8,
                "application/json");
            // --- FIM DA ALTERAÇÃO ---

            var response = await client.PostAsync($"{javaApiUrl}/api/users/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // ... (resto do método sem alterações)
                var token = GenerateJwtToken(loginDto.Login);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string login)
        {
            var jwtSecret = _configuration["Jwt:Secret"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, login)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class UserLoginDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}