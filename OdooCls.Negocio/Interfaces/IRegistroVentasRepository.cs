using OdooCls.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Core.Interfaces
{
    public interface IRegistroVentasRepository
    {
        public Task<bool> InsertTregv(RegistroVentas registroVentas);
        public Task<bool> InsertTregvD(RegistroVentasDetail registro);
        public Task<bool> InsertCtxC(int ejercicio, int mes, string Tipodoc, string nrodoc);
       
        /// <summary>
        /// Valida que el documento que se esta ingresando no exista en el registro de ventas 
        /// </summary>
        /// <param name="ejercicio">Año</param>
        /// <param name="mes">Mes</param>
        /// <param name="Tipodoc">Tipo Documento</param>
        /// <param name="nrodoc">Nro Documento</param>
        /// <returns></returns>
        public Task<bool> ValidarExistenciaDocumento(int ejercicio, int mes, string Tipodoc, string nrodoc);
        /// <summary>
        /// Valida estatus de Situacion contable
        /// </summary>
        /// <param name="ejercicio">Año</param>
        /// <param name="mes">Mes</param>
        /// <param name="stconta">Tipo</param>
        /// <returns></returns>
        public Task<bool> ValidarStatusRV(int ejercicio, int mes, string stconta);
        public Task<bool> ValidatTipoDoc(string tipo);
        public Task<bool> ValidaCliente(string codclie);
        public Task<bool> ValidaMoneda(int moneda);

    }
}
