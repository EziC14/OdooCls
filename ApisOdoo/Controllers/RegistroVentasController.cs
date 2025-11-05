using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OdooCls.Application.Dtos;
using OdooCls.Application.Services;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;

namespace OdooCls.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroVentasController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IRegistroVentasRepository registro;
        private readonly RegistroVentasServices registroVentasApplication;
        string? keys = "";
        private  string VALID_TOKEN="";
        public RegistroVentasController(IConfiguration configuration, IRegistroVentasRepository registro)
        {
            this.configuration = configuration;
            this.registro = registro;
            registroVentasApplication = new RegistroVentasServices(this.configuration, this.registro);
            this.keys = Convert.ToString(this.configuration["Authentication:ApiKey"]);
            VALID_TOKEN = this.keys;
        }
       
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateRegVtas([FromBody] RegistroVentasDto rvdto)
        {
            try
            {
                // 1. Verificación del Bearer Token en el encabezado Authorization
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer"))
                {
                    return Unauthorized(new { message = "Falta el token Bearer" });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();

                if (token != VALID_TOKEN)
                {
                    return Unauthorized(new { message = "Token no válido" });
                }

                // 2. Llamamos al servicio que contiene la lógica de negocio
                var response = await registroVentasApplication.CreateRegVtasAsync(rvdto);

                // 3. Si la respuesta tiene un código de estado 200 (éxito)
                if (response.HttpStatusCode == 200)
                {
                  // await registroVentasApplication.CreateRegVtasAsync(rvdto);

                    // Devuelves la respuesta con el código 200 OK, manteniendo toda la información
                    return Ok(response);
                }

                // 4. Si no es 200, devolvemos el código de estado adecuado con el ApiResponse completo
                return StatusCode(response.HttpStatusCode, response);
            }
            catch (Exception ex)
            {
                // En caso de excepción, devolvemos un error interno del servidor con código 500
                var errorResponse = new ApiResponse<RegistroVentasDto>(500, 500, $"Error interno del servidor: {ex.Message}");
                return StatusCode(500, errorResponse);
            }
    }


    }
}
