using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using OdooCls.Application.Dtos;
using OdooCls.Application.Mapper;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Application.Services
{
    public class RegistroComprasServices
    {
        private readonly IConfiguration configuration;
        string? library;
        private readonly IRegistroComprasRepository Registro;
        string? keys = "";
        public RegistroComprasServices(IConfiguration configuration,IRegistroComprasRepository registro)
        {
            this.configuration = configuration;
            this.library = this.configuration["Authentication:Library"];
            this.Registro = registro;
            this.keys = Convert.ToString(this.configuration["Authentication:ApiKey"]);
        }
        public async Task<ApiResponse<RegistroComprasDto>> CreateRegComAsync(RegistroComprasDto rcdto)
        {
            bool V_PerConta_Rc, V_PerConta_Conta, V_TipoDoc;
            bool Itregc = false;
            bool Valida = false;
            int ejercicio = rcdto.RCEJER, mes = rcdto.RCPERI;
            string td = rcdto.RCTDOC, tn = rcdto.RCNDOC;
            try
            {
                if (rcdto == null)
                {
                    return new ApiResponse<RegistroComprasDto>(400, 001, $"No se recibio datos en el Archivo");
                }
                V_PerConta_Conta = await Registro.ValidarStatusRC(ejercicio, mes, "CO");
                if (V_PerConta_Conta == false)
                {
                    return new ApiResponse<RegistroComprasDto>(400, 1002, $"El Periodo Contable esta Cerrado");
                }
                V_PerConta_Rc = await Registro.ValidarStatusRC(ejercicio, mes, "RC");
                if (V_PerConta_Rc == false)
                { return new ApiResponse<RegistroComprasDto>(400, 1003, $"El Registro de Compras se encuentra Cerrado"); }

                V_TipoDoc = await Registro.ValidatTipoDoc(rcdto.RCTDOC);
                if (V_TipoDoc == false)
                {
                    return new ApiResponse<RegistroComprasDto>(400, 1003, $"El Tipo de Documento {rcdto.RCTDOC} no existe en el maestr");
                }
                Valida = await Registro.ValidaMoneda(rcdto.RCMONE);
                if (Valida == false)
                {
                    return new ApiResponse<RegistroComprasDto>(400, 1006, $"La Moneda ingresa no existe");
                }
                if (string.IsNullOrWhiteSpace(rcdto.RCCPRO))
                {
                    return new ApiResponse<RegistroComprasDto>(400, 1007, "RCCPRO es obligatorio (código de proveedor)");
                }

                Valida = await Registro.ValidaProveedor(rcdto.RCCPRO);
                if (Valida == false)
                {
                    return new ApiResponse<RegistroComprasDto>(400, 1007, $"Proveedor no existe");
                }

                var documentoExiste = await Registro.ValidarExistenciaDocumento(ejercicio, mes, rcdto.RCTDOC, rcdto.RCNDOC);
                if (documentoExiste)
                {
                    return new ApiResponse<RegistroComprasDto>(400, 1008, $"El documento {rcdto.RCTDOC}-{rcdto.RCNDOC} ya existe en RC/CXP");
                }

                var peri = DateTime.Now.ToString("yyyyMM");
                var anio = DateTime.Now.Year;
                var meses = DateTime.Now.Month;
                int correla;
                try
                {
                    correla = Registro.GetNextCorr(peri);
                }
                catch (Exception exCorr)
                {
                    return new ApiResponse<RegistroComprasDto>(500, 500, $"Fallo en GetNextCorr (periodo={peri}): {exCorr.Message}");
                }
                string rcxp = "";
                switch (meses)
                {
                    case 10:
                        rcxp = anio.ToString() + "A" + correla.ToString("D5");
                        break;
                    case 11:
                        rcxp = anio.ToString() + "B" + correla.ToString("D5");
                        break;
                    case 12:
                        rcxp = anio.ToString() + "C" + correla.ToString("D5");
                        break;
                    default:
                        rcxp = anio.ToString() + meses.ToString() + correla.ToString("D5");
                        break;
                }

                rcdto.RCRCXP = rcxp;

                // Valores fijos requeridos por el sistema
                //rcdto.RCCVAL = "6011003";
                //rcdto.RCMVAL = "C";
                //rcdto.RCCIM1 = "4011001";
                //rcdto.RCMIM1 = "C";
                // rcdto.RCCPVT = "4212001";
                //rcdto.RCMPVT = "A";

                // CENTRO DE COSTOS FIJO PARA COMPRAS, SEGUN REQUERIMIENTO DE SISTEMA
                //rcdto.RCCOST = "014100002";

                // Clasificacion de B/S siuempre en 1 para compras RCCBSA

                rcdto.RCCBSA = "1";



                RegistroCompras compras = RegistroComprasMapper.DtoToEntity(rcdto);
                try
                {
                    Itregc = await Registro.InsertTregcAndCtxp(compras);
                }
                catch (Exception exIns)
                {
                    return new ApiResponse<RegistroComprasDto>(500, 500, $"Fallo en transacción RC/CXP: {exIns.Message}");
                }
                if (Itregc == true)
                {
                    return new ApiResponse<RegistroComprasDto>(200, 1000, $"Documentos Registrado en el RC");
                }
                else 
                {
                    return new ApiResponse<RegistroComprasDto>(400, 1001, $"no se pudo registrar en el registro de compras ");
                }


            }
            catch (Exception ex)
            {
                // En caso de error inesperado, capturamos la excepción y retornamos un error genérico 
                return new ApiResponse<RegistroComprasDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }

        }
    }
}
