using Microsoft.AspNetCore.Mvc;
using OdooCls.Application.Dtos;
using OdooCls.Application.Services;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;


namespace OdooCls.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroMovimientosController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IRegistroMovimientosRepository repo;
        private readonly RegistroMovimientosServices svc;
        private readonly string VALID_TOKEN;

        public RegistroMovimientosController(IConfiguration configuration, IRegistroMovimientosRepository repo)
        {
            this.configuration = configuration;
            this.repo = repo;
            this.svc = new RegistroMovimientosServices(configuration, repo);
            this.VALID_TOKEN = Convert.ToString(this.configuration["Authentication:ApiKey"]) ?? string.Empty;
        }

        /// <summary>
        /// Crea un nuevo movimiento de inventario con diferentes tipos
        /// Tipos soportados:
        /// - PEDIDO: Inserta en TMOVH + TMOVD + TPEDH + TPEDD
        /// - NOTA_CREDITO: Pendiente de implementar
        /// </summary>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] RegistroMovimientosDto dto)
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
                var errorResponse = new ApiResponse<RegistroMovimientosDto>(500, 500, $"Error interno del servidor: {ex.Message}");
                return StatusCode(500, errorResponse);
            }
        }
        [HttpGet]
        [Route("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                Success = true,
                Message = "API de Movimientos funcionando correctamente",
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Version = "1.0.0",
                TiposSoportados = new[] { "PEDIDO", "NOTA_CREDITO (pendiente)" }
            });
        }
    }
}
