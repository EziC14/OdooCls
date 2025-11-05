using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Application.Dtos
{
    public class RegistroVentasDetailDto
    {
        public int RVEJER { get; set; } = 0;
        public int RVPERI { get; set; } = 0;
        public string RVTDOC { get; set; } = "";
        public string RVNDOC { get; set; } = "";
        public int RVSECU { get; set; } = 0;
        public string RVDCTA { get; set; } = "";
        public string RVDCCO { get; set; } = "";
        public decimal RVDIMP { get; set; } = 0;
    }
}
