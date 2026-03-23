using Microsoft.AspNetCore.Mvc;
using OdooCls.Application.Dtos;
using OdooCls.Application.Services;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;

namespace OdooCls.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroAlmacenesController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IRegistroAlmacenesRepository repo;
        private readonly RegistroAlmacenesServices svc;
        private readonly string VALID_TOKEN;

        public RegistroAlmacenesController(IConfiguration configuration, IRegistroAlmacenesRepository repo)
        {
            this.configuration = configuration;
            this.repo = repo;
            this.svc = new RegistroAlmacenesServices(configuration, repo);
            this.VALID_TOKEN = Convert.ToString(this.configuration["Authentication:ApiKey"]) ?? string.Empty;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] RegistroAlmacenesDto dto)
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
                var errorResponse = new ApiResponse<RegistroAlmacenesDto>(500, 500, $"Error interno del servidor: {ex.Message}");
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] RegistroAlmacenesDto dto)
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
                var errorResponse = new ApiResponse<RegistroAlmacenesDto>(500, 500, $"Error interno del servidor: {ex.Message}");
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer"))
                return Unauthorized(new { message = "Falta el token Bearer" });

            var token = authHeader.Substring("Bearer ".Length).Trim();
            if (token != VALID_TOKEN)
                return Unauthorized(new { message = "Token no válido" });

            var response = await svc.GetAllAsync(page, pageSize);
            return StatusCode(response.HttpStatusCode, response);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer"))
                return Unauthorized(new { message = "Falta el token Bearer" });

            var token = authHeader.Substring("Bearer ".Length).Trim();
            if (token != VALID_TOKEN)
                return Unauthorized(new { message = "Token no válido" });

            var response = await svc.GetByIdAsync(id);
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
