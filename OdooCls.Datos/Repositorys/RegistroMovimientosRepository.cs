using Microsoft.Extensions.Configuration;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;
using System.Data;
using System.Data.Odbc;

namespace OdooCls.Infrastucture.Repositorys
{
    public class RegistroMovimientosRepository : IRegistroMovimientosRepository
    {
        private readonly IConfiguration configuration;
        string? library;
        string? connectionString;

        public RegistroMovimientosRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            library = this.configuration["Authentication:Library"];
            connectionString = this.configuration["ConnectionStrings:ERPConexion"];
        }

        public async Task<bool> ExisteMovimiento(int ejercicio, int periodo, string almacen, int comprobante)
        {
            string q = $@"select count(1) from {library}.tmovh 
                          where MHEJER=? and MHPERI=? and MHALMA=? and MHCOMP=?";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(q, cn);
                await cn.OpenAsync();
                cmd.Parameters.AddWithValue("@MHEJER", ejercicio);
                cmd.Parameters.AddWithValue("@MHPERI", periodo);
                cmd.Parameters.AddWithValue("@MHALMA", almacen);
                cmd.Parameters.AddWithValue("@MHCOMP", comprobante);
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> InsertTmovh(RegistroMovimiento m)
        {
            string query = $@"insert into {library}.tmovh 
                (MHALMA, MHASTO, MHCHOF, MHCMOV, MHCOMP, MHEJER, MHFECH, MHFEIN, MHFEMD, 
                 MHHOIN, MHHOMD, MHHRE1, MHHRE2, MHHRE3, MHPERI, MHREF1, MHREF2, MHREF3, 
                 MHREF4, MHREF5, MHSITD, MHSITU, MHTMOV, MHUSEA, MHUSER, MHUSIN, MHUSMD, MHVEHI)
                values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@MHALMA", m.MHALMA);
                cmd.Parameters.AddWithValue("@MHASTO", m.MHASTO);
                cmd.Parameters.AddWithValue("@MHCHOF", m.MHCHOF);
                cmd.Parameters.AddWithValue("@MHCMOV", m.MHCMOV);
                cmd.Parameters.AddWithValue("@MHCOMP", m.MHCOMP);
                cmd.Parameters.AddWithValue("@MHEJER", m.MHEJER);
                cmd.Parameters.AddWithValue("@MHFECH", m.MHFECH);
                cmd.Parameters.AddWithValue("@MHFEIN", m.MHFEIN);
                cmd.Parameters.AddWithValue("@MHFEMD", m.MHFEMD);
                cmd.Parameters.AddWithValue("@MHHOIN", m.MHHOIN);
                cmd.Parameters.AddWithValue("@MHHOMD", m.MHHOMD);
                cmd.Parameters.AddWithValue("@MHHRE1", m.MHHRE1);
                cmd.Parameters.AddWithValue("@MHHRE2", m.MHHRE2);
                cmd.Parameters.AddWithValue("@MHHRE3", m.MHHRE3);
                cmd.Parameters.AddWithValue("@MHPERI", m.MHPERI);
                cmd.Parameters.AddWithValue("@MHREF1", m.MHREF1);
                cmd.Parameters.AddWithValue("@MHREF2", m.MHREF2);
                cmd.Parameters.AddWithValue("@MHREF3", m.MHREF3);
                cmd.Parameters.AddWithValue("@MHREF4", m.MHREF4);
                cmd.Parameters.AddWithValue("@MHREF5", m.MHREF5);
                cmd.Parameters.AddWithValue("@MHSITD", m.MHSITD);
                cmd.Parameters.AddWithValue("@MHSITU", m.MHSITU);
                cmd.Parameters.AddWithValue("@MHTMOV", m.MHTMOV);
                cmd.Parameters.AddWithValue("@MHUSEA", m.MHUSEA);
                cmd.Parameters.AddWithValue("@MHUSER", m.MHUSER);
                cmd.Parameters.AddWithValue("@MHUSIN", m.MHUSIN);
                cmd.Parameters.AddWithValue("@MHUSMD", m.MHUSMD);
                cmd.Parameters.AddWithValue("@MHVEHI", m.MHVEHI);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> InsertTmovd(RegistroMovimientoDetail d)
        {
            string query = $@"insert into {library}.tmovd 
                (MDACTI, MDALMA, MDCANA, MDCANR, MDCMOV, MDCOAR, MDCOEP, MDCOMO, MDCOMP, MDCORR, 
                 MDCOSP, MDCOST, MDCUEA, MDCUER, MDCUNA, MDCUNR, MDDRE0, MDDRE1, MDDRE2, MDDRE4, 
                 MDDRE5, MDEJER, MDFECH, MDFVTO, MDLOTE, MDMONO, MDPERI, MDSITD, MDSITU, MDTCAM, 
                 MDTCMO, MDTMOV, MDTOEC, MDTOTC, MDUMER)
                values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@MDACTI", d.MDACTI);
                cmd.Parameters.AddWithValue("@MDALMA", d.MDALMA);
                cmd.Parameters.AddWithValue("@MDCANA", d.MDCANA);
                cmd.Parameters.AddWithValue("@MDCANR", d.MDCANR);
                cmd.Parameters.AddWithValue("@MDCMOV", d.MDCMOV);
                cmd.Parameters.AddWithValue("@MDCOAR", d.MDCOAR);
                cmd.Parameters.AddWithValue("@MDCOEP", d.MDCOEP);
                cmd.Parameters.AddWithValue("@MDCOMO", d.MDCOMO);
                cmd.Parameters.AddWithValue("@MDCOMP", d.MDCOMP);
                cmd.Parameters.AddWithValue("@MDCORR", d.MDCORR);
                cmd.Parameters.AddWithValue("@MDCOSP", d.MDCOSP);
                cmd.Parameters.AddWithValue("@MDCOST", d.MDCOST);
                cmd.Parameters.AddWithValue("@MDCUEA", d.MDCUEA);
                cmd.Parameters.AddWithValue("@MDCUER", d.MDCUER);
                cmd.Parameters.AddWithValue("@MDCUNA", d.MDCUNA);
                cmd.Parameters.AddWithValue("@MDCUNR", d.MDCUNR);
                cmd.Parameters.AddWithValue("@MDDRE0", d.MDDRE0);
                cmd.Parameters.AddWithValue("@MDDRE1", d.MDDRE1);
                cmd.Parameters.AddWithValue("@MDDRE2", d.MDDRE2);
                cmd.Parameters.AddWithValue("@MDDRE4", d.MDDRE4);
                cmd.Parameters.AddWithValue("@MDDRE5", d.MDDRE5);
                cmd.Parameters.AddWithValue("@MDEJER", d.MDEJER);
                cmd.Parameters.AddWithValue("@MDFECH", d.MDFECH);
                cmd.Parameters.AddWithValue("@MDFVTO", d.MDFVTO);
                cmd.Parameters.AddWithValue("@MDLOTE", d.MDLOTE);
                cmd.Parameters.AddWithValue("@MDMONO", d.MDMONO);
                cmd.Parameters.AddWithValue("@MDPERI", d.MDPERI);
                cmd.Parameters.AddWithValue("@MDSITD", d.MDSITD);
                cmd.Parameters.AddWithValue("@MDSITU", d.MDSITU);
                cmd.Parameters.AddWithValue("@MDTCAM", d.MDTCAM);
                cmd.Parameters.AddWithValue("@MDTCMO", d.MDTCMO);
                cmd.Parameters.AddWithValue("@MDTMOV", d.MDTMOV);
                cmd.Parameters.AddWithValue("@MDTOEC", d.MDTOEC);
                cmd.Parameters.AddWithValue("@MDTOTC", d.MDTOTC);
                cmd.Parameters.AddWithValue("@MDUMER", d.MDUMER);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> InsertTpedh(RegistroPedido p)
        {
            string query = $@"insert into {library}.tpedh 
                (PHALMA, PHAUTO, PHCLIE, PHCLVT, PHCOST, PHCPAG, PHDIRC, PHDISC, PHDTIA, PHDTOA, 
                 PHEDS2, PHEIGV, PHEPVT, PHEVVA, PHEVVI, PHFECE, PHFECP, PHFEIN, PHFEMD, PHFL01, 
                 PHFL03, PHFL06, PHFL07, PHFL08, PHFL09, PHFL10, PHFL11, PHFL12, PHHOIN, PHHOMD, 
                 PHHORP, PHMONE, PHNDS2, PHNIDE, PHNIGV, PHNOMC, PHNPVT, PHNUME, PHNVVA, PHNVVI, 
                 PHORIG, PHPERE, PHPERN, PHPVTA, PHREF1, PHREF2, PHREF3, PHREF4, PHREF6, PHREF7, 
                 PHREF8, PHREF9, PHREFA, PHRUBR, PHRUCC, PHSITD, PHSITU, PHTCAM, PHTDOC, PHTIDE, 
                 PHTVTA, PHUSAP, PHUSIN, PHUSMD, PHZONA)
                values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PHALMA", p.PHALMA);
                cmd.Parameters.AddWithValue("@PHAUTO", p.PHAUTO);
                cmd.Parameters.AddWithValue("@PHCLIE", p.PHCLIE);
                cmd.Parameters.AddWithValue("@PHCLVT", p.PHCLVT);
                cmd.Parameters.AddWithValue("@PHCOST", p.PHCOST);
                cmd.Parameters.AddWithValue("@PHCPAG", p.PHCPAG);
                cmd.Parameters.AddWithValue("@PHDIRC", p.PHDIRC);
                cmd.Parameters.AddWithValue("@PHDISC", p.PHDISC);
                cmd.Parameters.AddWithValue("@PHDTIA", p.PHDTIA);
                cmd.Parameters.AddWithValue("@PHDTOA", p.PHDTOA);
                cmd.Parameters.AddWithValue("@PHEDS2", p.PHEDS2);
                cmd.Parameters.AddWithValue("@PHEIGV", p.PHEIGV);
                cmd.Parameters.AddWithValue("@PHEPVT", p.PHEPVT);
                cmd.Parameters.AddWithValue("@PHEVVA", p.PHEVVA);
                cmd.Parameters.AddWithValue("@PHEVVI", p.PHEVVI);
                cmd.Parameters.AddWithValue("@PHFECE", p.PHFECE);
                cmd.Parameters.AddWithValue("@PHFECP", p.PHFECP);
                cmd.Parameters.AddWithValue("@PHFEIN", p.PHFEIN);
                cmd.Parameters.AddWithValue("@PHFEMD", p.PHFEMD);
                cmd.Parameters.AddWithValue("@PHFL01", p.PHFL01);
                cmd.Parameters.AddWithValue("@PHFL03", p.PHFL03);
                cmd.Parameters.AddWithValue("@PHFL06", p.PHFL06);
                cmd.Parameters.AddWithValue("@PHFL07", p.PHFL07);
                cmd.Parameters.AddWithValue("@PHFL08", p.PHFL08);
                cmd.Parameters.AddWithValue("@PHFL09", p.PHFL09);
                cmd.Parameters.AddWithValue("@PHFL10", p.PHFL10);
                cmd.Parameters.AddWithValue("@PHFL11", p.PHFL11);
                cmd.Parameters.AddWithValue("@PHFL12", p.PHFL12);
                cmd.Parameters.AddWithValue("@PHHOIN", p.PHHOIN);
                cmd.Parameters.AddWithValue("@PHHOMD", p.PHHOMD);
                cmd.Parameters.AddWithValue("@PHHORP", p.PHHORP);
                cmd.Parameters.AddWithValue("@PHMONE", p.PHMONE);
                cmd.Parameters.AddWithValue("@PHNDS2", p.PHNDS2);
                cmd.Parameters.AddWithValue("@PHNIDE", p.PHNIDE);
                cmd.Parameters.AddWithValue("@PHNIGV", p.PHNIGV);
                cmd.Parameters.AddWithValue("@PHNOMC", p.PHNOMC);
                cmd.Parameters.AddWithValue("@PHNPVT", p.PHNPVT);
                cmd.Parameters.AddWithValue("@PHNUME", p.PHNUME);
                cmd.Parameters.AddWithValue("@PHNVVA", p.PHNVVA);
                cmd.Parameters.AddWithValue("@PHNVVI", p.PHNVVI);
                cmd.Parameters.AddWithValue("@PHORIG", p.PHORIG);
                cmd.Parameters.AddWithValue("@PHPERE", p.PHPERE);
                cmd.Parameters.AddWithValue("@PHPERN", p.PHPERN);
                cmd.Parameters.AddWithValue("@PHPVTA", p.PHPVTA);
                cmd.Parameters.AddWithValue("@PHREF1", p.PHREF1);
                cmd.Parameters.AddWithValue("@PHREF2", p.PHREF2);
                cmd.Parameters.AddWithValue("@PHREF3", p.PHREF3);
                cmd.Parameters.AddWithValue("@PHREF4", p.PHREF4);
                cmd.Parameters.AddWithValue("@PHREF6", p.PHREF6);
                cmd.Parameters.AddWithValue("@PHREF7", p.PHREF7);
                cmd.Parameters.AddWithValue("@PHREF8", p.PHREF8);
                cmd.Parameters.AddWithValue("@PHREF9", p.PHREF9);
                cmd.Parameters.AddWithValue("@PHREFA", p.PHREFA);
                cmd.Parameters.AddWithValue("@PHRUBR", p.PHRUBR);
                cmd.Parameters.AddWithValue("@PHRUCC", p.PHRUCC);
                cmd.Parameters.AddWithValue("@PHSITD", p.PHSITD);
                cmd.Parameters.AddWithValue("@PHSITU", p.PHSITU);
                cmd.Parameters.AddWithValue("@PHTCAM", p.PHTCAM);
                cmd.Parameters.AddWithValue("@PHTDOC", p.PHTDOC);
                cmd.Parameters.AddWithValue("@PHTIDE", p.PHTIDE);
                cmd.Parameters.AddWithValue("@PHTVTA", p.PHTVTA);
                cmd.Parameters.AddWithValue("@PHUSAP", p.PHUSAP);
                cmd.Parameters.AddWithValue("@PHUSIN", p.PHUSIN);
                cmd.Parameters.AddWithValue("@PHUSMD", p.PHUSMD);
                cmd.Parameters.AddWithValue("@PHZONA", p.PHZONA);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> InsertTpedd(RegistroPedidoDetail d)
        {
            string query = $@"insert into {library}.tpedd 
                (PDARTI, PDCAN1, PDCANT, PDCEQU, PDCLIE, PDCMOA, PDEDS2, PDEIGV, PDEPVT, PDEVVA, 
                 PDEVVI, PDFABO, PDFBMA, PDFECF, PDFECG, PDFECP, PDFECV, PDFEXF, PDFVTA, PDGUIA, 
                 PDHORF, PDHORG, PDHOXF, PDITEM, PDLOTE, PDLPCO, PDMONE, PDNART, PDNCOT, PDNDS2, 
                 PDNIGV, PDNPVT, PDNUME, PDNVVA, PDNVVI, PDOBSA, PDPVTA, PDREF0, PDREF1, PDREF2, 
                 PDREF3, PDREF4, PDREF5, PDREF7, PDREF9, PDREFA, PDRFDE, PDSCOT, PDSECC, PDSECU, 
                 PDSERI, PDSITD, PDTDOC, PDTVTA, PDUNIT, PDUNVT, PDUSAF, PDUSAG, PDUSXF, PDVALE, 
                 PDZONA, REQNRO)
                values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            try
            {
                using var cn = new OdbcConnection(connectionString);
                using var cmd = new OdbcCommand(query, cn);
                await cn.OpenAsync();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PDARTI", d.PDARTI);
                cmd.Parameters.AddWithValue("@PDCAN1", d.PDCAN1);
                cmd.Parameters.AddWithValue("@PDCANT", d.PDCANT);
                cmd.Parameters.AddWithValue("@PDCEQU", d.PDCEQU);
                cmd.Parameters.AddWithValue("@PDCLIE", d.PDCLIE);
                cmd.Parameters.AddWithValue("@PDCMOA", d.PDCMOA);
                cmd.Parameters.AddWithValue("@PDEDS2", d.PDEDS2);
                cmd.Parameters.AddWithValue("@PDEIGV", d.PDEIGV);
                cmd.Parameters.AddWithValue("@PDEPVT", d.PDEPVT);
                cmd.Parameters.AddWithValue("@PDEVVA", d.PDEVVA);
                cmd.Parameters.AddWithValue("@PDEVVI", d.PDEVVI);
                cmd.Parameters.AddWithValue("@PDFABO", d.PDFABO);
                cmd.Parameters.AddWithValue("@PDFBMA", d.PDFBMA);
                cmd.Parameters.AddWithValue("@PDFECF", d.PDFECF);
                cmd.Parameters.AddWithValue("@PDFECG", d.PDFECG);
                cmd.Parameters.AddWithValue("@PDFECP", d.PDFECP);
                cmd.Parameters.AddWithValue("@PDFECV", d.PDFECV);
                cmd.Parameters.AddWithValue("@PDFEXF", d.PDFEXF);
                cmd.Parameters.AddWithValue("@PDFVTA", d.PDFVTA);
                cmd.Parameters.AddWithValue("@PDGUIA", d.PDGUIA);
                cmd.Parameters.AddWithValue("@PDHORF", d.PDHORF);
                cmd.Parameters.AddWithValue("@PDHORG", d.PDHORG);
                cmd.Parameters.AddWithValue("@PDHOXF", d.PDHOXF);
                cmd.Parameters.AddWithValue("@PDITEM", d.PDITEM);
                cmd.Parameters.AddWithValue("@PDLOTE", d.PDLOTE);
                cmd.Parameters.AddWithValue("@PDLPCO", d.PDLPCO);
                cmd.Parameters.AddWithValue("@PDMONE", d.PDMONE);
                cmd.Parameters.AddWithValue("@PDNART", d.PDNART);
                cmd.Parameters.AddWithValue("@PDNCOT", d.PDNCOT);
                cmd.Parameters.AddWithValue("@PDNDS2", d.PDNDS2);
                cmd.Parameters.AddWithValue("@PDNIGV", d.PDNIGV);
                cmd.Parameters.AddWithValue("@PDNPVT", d.PDNPVT);
                cmd.Parameters.AddWithValue("@PDNUME", d.PDNUME);
                cmd.Parameters.AddWithValue("@PDNVVA", d.PDNVVA);
                cmd.Parameters.AddWithValue("@PDNVVI", d.PDNVVI);
                cmd.Parameters.AddWithValue("@PDOBSA", d.PDOBSA);
                cmd.Parameters.AddWithValue("@PDPVTA", d.PDPVTA);
                cmd.Parameters.AddWithValue("@PDREF0", d.PDREF0);
                cmd.Parameters.AddWithValue("@PDREF1", d.PDREF1);
                cmd.Parameters.AddWithValue("@PDREF2", d.PDREF2);
                cmd.Parameters.AddWithValue("@PDREF3", d.PDREF3);
                cmd.Parameters.AddWithValue("@PDREF4", d.PDREF4);
                cmd.Parameters.AddWithValue("@PDREF5", d.PDREF5);
                cmd.Parameters.AddWithValue("@PDREF7", d.PDREF7);
                cmd.Parameters.AddWithValue("@PDREF9", d.PDREF9);
                cmd.Parameters.AddWithValue("@PDREFA", d.PDREFA);
                cmd.Parameters.AddWithValue("@PDRFDE", d.PDRFDE);
                cmd.Parameters.AddWithValue("@PDSCOT", d.PDSCOT);
                cmd.Parameters.AddWithValue("@PDSECC", d.PDSECC);
                cmd.Parameters.AddWithValue("@PDSECU", d.PDSECU);
                cmd.Parameters.AddWithValue("@PDSERI", d.PDSERI);
                cmd.Parameters.AddWithValue("@PDSITD", d.PDSITD);
                cmd.Parameters.AddWithValue("@PDTDOC", d.PDTDOC);
                cmd.Parameters.AddWithValue("@PDTVTA", d.PDTVTA);
                cmd.Parameters.AddWithValue("@PDUNIT", d.PDUNIT);
                cmd.Parameters.AddWithValue("@PDUNVT", d.PDUNVT);
                cmd.Parameters.AddWithValue("@PDUSAF", d.PDUSAF);
                cmd.Parameters.AddWithValue("@PDUSAG", d.PDUSAG);
                cmd.Parameters.AddWithValue("@PDUSXF", d.PDUSXF);
                cmd.Parameters.AddWithValue("@PDVALE", d.PDVALE);
                cmd.Parameters.AddWithValue("@PDZONA", d.PDZONA);
                cmd.Parameters.AddWithValue("@REQNRO", d.REQNRO);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
