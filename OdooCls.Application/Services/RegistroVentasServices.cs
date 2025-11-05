using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using OdooCls.Application.Dtos;
using OdooCls.Application.Mapper;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Application.Services
{
    public   class RegistroVentasServices
    {
        private readonly IConfiguration configuration;
        string? library;
        private readonly IRegistroVentasRepository Registro;
        string? keys = "";

        public RegistroVentasServices(IConfiguration configuration, IRegistroVentasRepository registro)
        {
            this.configuration = configuration;
            this.library = this.configuration["Authentication:Library"]; 
            this.Registro = registro;
            this.keys = Convert.ToString(this.configuration["Authentication:ApiKey"]);
        }
        public async Task<ApiResponse<RegistroVentasDto>> CreateRegVtasAsync(RegistroVentasDto rvdto)
        {        
            bool V_PerConta_Rv, V_PerConta_Conta,V_TipoDoc;
            bool Itregv = false;
            //bool Itregvd = false;

            
            bool Valida=false;
            int ejercicio = rvdto.RVEJER, mes = rvdto.RVPERI;
            string td = rvdto.RVTDOC, tn = rvdto.RVNDOC;
            try
            {
                if (rvdto == null)
                {
                    return new ApiResponse<RegistroVentasDto>(400, 001, "No se recibio datos en el Archivo");
                }
                V_PerConta_Conta = await Registro.ValidarStatusRV(ejercicio, mes, "CO");
                if (V_PerConta_Conta == false)
                {
                    return new ApiResponse<RegistroVentasDto>(400, 1002, $"El Periodo Contable esta Cerrado");
                }

                V_PerConta_Rv = await Registro.ValidarStatusRV(ejercicio, mes, "RV");
                if (V_PerConta_Rv == false)
                {
                    return new ApiResponse<RegistroVentasDto>(400, 1003, $"El Registro de ventas se encuentra Cerrado");
                }

                //Validar que exista el Tipo de documento en el Maestro
                V_TipoDoc = await Registro.ValidatTipoDoc(rvdto.RVTDOC);
                if (V_TipoDoc == false)
                {
                    return new ApiResponse<RegistroVentasDto>(400, 1004, $"El Tipo de Documento {rvdto.RVTDOC} no existe en el maestro");
                }

                //Validar Existencia del documento en el Registro de ventas
                Valida = await Registro.ValidarExistenciaDocumento(ejercicio, mes, td, tn);
                if (Valida == false){
                    return new ApiResponse<RegistroVentasDto>(400, 1005, $"Documento ya existe en el Registro de Ventas");
                }

                Valida = await Registro.ValidaMoneda(rvdto.RVMONE);
                if (Valida == false)
                {
                    return new ApiResponse<RegistroVentasDto>(400, 1006, $"La Moneda ingresa no existe");
                }

                Valida = await Registro.ValidaCliente(rvdto.RVCLIE);
                if (Valida == false)
                {
                    return new ApiResponse<RegistroVentasDto>(400, 1007, $"Cliente no existe");
                }


             

                RegistroVentas ventas = RegistroVentasMapper.DtoToEntity(rvdto);
                Itregv = await Registro.InsertTregv(ventas);
                if (Itregv == true)
                {
                    await Registro.InsertCtxC(ejercicio, mes, td, tn);
                    return new ApiResponse<RegistroVentasDto>(200, 1000, $"Documento registrado en el Registro de Ventas");
                }
                else
                {
                    return new ApiResponse<RegistroVentasDto>(400, 1001, $"No se pudo registrar el documento en el Registro de Ventas");
                }
            }
            catch (Exception ex)
            { // En caso de error inesperado, capturamos la excepción y retornamos un error genérico 
                return new ApiResponse<RegistroVentasDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }

        }
    }
}
