using Microsoft.AspNetCore.Mvc;
using OdooCls.Application.Dtos;
using OdooCls.Application.Services;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;

namespace OdooCls.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroClientesController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IRegistroClientesRepository repo;
        private readonly RegistroClientesServices svc;
        private readonly string VALID_TOKEN;

        public RegistroClientesController(IConfiguration configuration, IRegistroClientesRepository repo)
        {
            this.configuration = configuration;
            this.repo = repo;
            this.svc = new RegistroClientesServices(configuration, repo);
            this.VALID_TOKEN = Convert.ToString(this.configuration["Authentication:ApiKey"]) ?? string.Empty;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] RegistroClientesDto dto)
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer"))
                    return Unauthorized(new { message = "Falta el token Bearer" });

                var token = authHeader.Substring("Bearer ".Length).Trim();
                if (token != VALID_TOKEN)
                    return Unauthorized(new { message = "Token no válido" });

                var response = await svc.CreateAsync(dto);
                if (response.HttpStatusCode == 200)
                    return Ok(response);
                return StatusCode(response.HttpStatusCode, response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<RegistroClientesDto>(500, 500, $"Error interno del servidor: {ex.Message}");
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] RegistroClientesDto dto)
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer"))
                    return Unauthorized(new { message = "Falta el token Bearer" });

                var token = authHeader.Substring("Bearer ".Length).Trim();
                if (token != VALID_TOKEN)
                    return Unauthorized(new { message = "Token no válido" });

                var response = await svc.UpdateAsync(dto);
                if (response.HttpStatusCode == 200)
                    return Ok(response);
                return StatusCode(response.HttpStatusCode, response);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<RegistroClientesDto>(500, 500, $"Error interno del servidor: {ex.Message}");
                return StatusCode(500, errorResponse);
            }
        }
    }
}
