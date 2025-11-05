using OdooCls.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Core.Interfaces
{
    public interface IRegistroComprasRepository
    {
        public int GetNextCorr(string periodo);
        public Task<bool> InsertTregc(RegistroCompras registro);
        public Task<bool> InsertTregcd(RegistroComprasDetail registro);
        public Task<bool> ValidatTipoDoc(string tipo);
        public Task<bool> ValidaMoneda(int moneda);
        public Task<bool> ValidarExistenciaDocumento(int ejercicio, int mes, string Tipodoc, string nrodoc);
        public Task<bool> ValidarStatusRC(int ejercicio, int mes, string stconta);
        public Task<bool> ValidaProveedor(string codprov);
    }
}
