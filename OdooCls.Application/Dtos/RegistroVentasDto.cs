using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Application.Dtos
{
    public class RegistroVentasDto
    {
        public int RVEJER { get; set; } = 0;
        public int RVPERI { get; set; } = 0;

        [StringLength(2)]
        public string RVTDOC { get; set; } = "";
        [StringLength(15)]
        public string RVNDOC { get; set; } = "";
        public int RVFECH { get; set; } = 0;
        [StringLength(10)]
        public string RVCCLI { get; set; } = "";
        [StringLength(40)]
        public string RVCLIE { get; set; } = "";
        public int RVMONE { get; set; } = 0;
        public decimal RVTCAM { get; set; } = 0;
        public decimal RVVALV { get; set; } = 0;
        [StringLength(1)]
        public string RVMVAL { get; set; } = "";
        public decimal RVVALI { get; set; } = 0;
        [StringLength(1)]
        public string RVMVAI { get; set; } = "";        
        public decimal RVDSCT { get; set; } = 0;
        [StringLength(1)]
        public string RVMDSC { get; set; } = "";
        public decimal RVIGV { get; set; } = 0;
        [StringLength(1)]
        public string RVMIGV { get; set; } = "";
        public decimal RVPVTA { get; set; } = 0;
        public string RVCPVT { get; set; } = "";
        [StringLength(1)]
        public string RVMPVT { get; set; } = "";
        public int RVCONC { get; set; } = 0;
        [StringLength(2)]
        public string RVTREF { get; set; } = "";
        [StringLength(15)]
        public string RVNREF { get; set; } = "";
        [StringLength(1)]
        public string RVGRAB { get; set; } = "";
        public int RVFPRO { get; set; } = 0;
        public int RVHPRO { get; set; } = 0;
        public int RVFEVE { get; set; } = 0;
        public string RVNDOM { get; set; } = "";
        public string RVCPAG { get; set; } = "";
        public string RVRUC { get; set; } = "";
        [StringLength(2)]
        public string RVSITU { get; set; } = "";
        [StringLength(15)]
        public string RVCOST { get; set; } = "";
        [StringLength(3)]
        public string RVCVEN { get; set; } = "";
        [StringLength(10)]
        public string RVUSIN { get; set; } = "";
        public int RVFEIN { get; set; } = 0;
        public int RVHOIN { get; set; } = 0;
        public List<RegistroVentasDetailDto>? detalle { get; set; } = null;
      
    }
}
