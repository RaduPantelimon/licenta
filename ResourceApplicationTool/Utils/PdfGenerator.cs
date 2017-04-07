using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using NReco.PdfGenerator;
using NReco;

namespace ResourceApplicationTool.Utils
{
    public class PdfGenerator
    {
        public static byte[] ConvertHtmlToPDF(string htmlCode, string header = null)
        {
            NReco.PdfGenerator.HtmlToPdfConverter nRecohtmltoPdfObj = new NReco.PdfGenerator.HtmlToPdfConverter();
            nRecohtmltoPdfObj.Orientation = PageOrientation.Portrait;
            //nRecohtmltoPdfObj.PageFooterHtml = CreatePDFFooter();
            //nRecohtmltoPdfObj.PageHeaderHtml = CreatePDFHeader(header);
            nRecohtmltoPdfObj.CustomWkHtmlArgs = " --load-media-error-handling ignore --disable-smart-shrinking --dpi 200 --margin-bottom 10 --margin-top 25 --header-spacing 10 --margin-left 20 --margin-right 20";

            return nRecohtmltoPdfObj.GeneratePdf(/*CreatePDFScript() + */"<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'/></head><body>" + htmlCode + "</body></html>");
            //return nRecohtmltoPdfObj.GeneratePdf(htmlCode);
        }
    }
}