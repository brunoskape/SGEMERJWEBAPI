using Microsoft.Reporting.WebForms;
using SGEMERJWEB.Entidade.Relatorios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace SGEMERJWEB.WebAPI.Util
{
    public class Uteis
    {
        ReportViewer reportVW = new ReportViewer();
        
        public string gerarArquivoPDF(string nomeArquivoPDF, string nomeRelatorio, List<FuncionariosAtivos> dataTableParameter = null, ReportParameter[] parametros = null, string extensaoArquivo = null)
        {
            byte[] bytes;
            string dirTempFisico = HttpContext.Current.Server.MapPath(@"~/Reports");
            string dirTempLogico = HttpContext.Current.Server.MapPath(@"~/temp/");
            Warning[] warnings;
            string[] streamIds;
            string contentType;
            string encoding = "UTF-8";
            string extension;
            string deviceInfo = "<DeviceInfo>" +
                    $"  <OutputFormat>{extensaoArquivo.ToUpper()}</OutputFormat>" +
                    //"  <PageWidth>8.27in</PageWidth>" +          ----- Medida comentada para padrão A4
                    //"  <PageHeight>11.69in</PageHeight>" +
                    "  <PageWidth>15.75in</PageWidth>" +
                    "  <PageHeight>8.27in</PageHeight>" +
                    "  <MarginTop>0.25in</MarginTop>" +
                    "  <MarginLeft>0in</MarginLeft>" +
                    "  <MarginRight>0.4in</MarginRight>" +
                    "  <MarginBottom>0in</MarginBottom>" +
                    "  <EmbedFonts>None</EmbedFonts>" +
                    "</DeviceInfo>";

            try
            {
                reportVW.Reset();
                reportVW.LocalReport.DataSources.Clear();
                reportVW.ProcessingMode = ProcessingMode.Local;
                reportVW.LocalReport.ReportPath = dirTempFisico + $"\\{nomeRelatorio}.rdlc";
                reportVW.ShowParameterPrompts = false;

                if (dataTableParameter.Count != 0)
                {
                    reportVW.LocalReport.DataSources.Add(new ReportDataSource("dsFuncionariosAtivos", dataTableParameter));
                }

                if (parametros != null)
                {
                    reportVW.LocalReport.SetParameters(parametros);
                }

                string data = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();

                switch (extensaoArquivo)
                {
                    case "xls":
                        {
                            bytes = reportVW.LocalReport.Render("EXCEL", deviceInfo, out contentType, out encoding, out extension, out streamIds, out warnings);

                            using (var fs = new FileStream(dirTempLogico + nomeArquivoPDF + "_" + data + "." + extensaoArquivo, FileMode.Create))
                            {
                                fs.Write(bytes, 0, bytes.Length);
                                fs.Close();
                            };
                        }
                    break;
                        
                    case "pdf":
                        {
                            bytes = reportVW.LocalReport.Render("PDF", deviceInfo, out contentType, out encoding, out extension, out streamIds, out warnings);

                            using (var fs = new FileStream(dirTempLogico + nomeArquivoPDF + "_" + data + "." + extensaoArquivo, FileMode.Create)) {
                                fs.Write(bytes, 0, bytes.Length);
                                fs.Close();
                            };
                        }
                    break;
                }

                return nomeArquivoPDF + "_" + data + "." + extensaoArquivo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}