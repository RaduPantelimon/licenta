﻿using System;
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
            nRecohtmltoPdfObj.PageFooterHtml = CreatePDFFooter();
            nRecohtmltoPdfObj.PageHeaderHtml = /*CreatePDFHeader(*/header;/*);*/
            nRecohtmltoPdfObj.CustomWkHtmlArgs = " --load-media-error-handling ignore --disable-smart-shrinking --dpi 200 --margin-bottom 10 --margin-top 45 --header-spacing 10 --margin-left 20 --margin-right 20";

            return nRecohtmltoPdfObj.GeneratePdf(/*CreatePDFScript() + */"<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'/></head><body>" + htmlCode + "</body></html>");
        }

        public static string CreatePDFFooter()
        {
            string footer = "<div>Page: <span class=\"page\"></span></div>"; ;
            return footer;
        }
        public static string CreatePDFHeader(string header = null)
        {
            string headerHtml =
           "<div style='width:100%; margin-top:1em; display:block; height:130px;'>" +
               "<img style='float:right; display:inline-block' width='125' height='100' src='" + "file:///" + System.Web.HttpContext.Current.Server.MapPath("~/Content/Pictures/employees-icon.jpg") + "' />" +
           "</div>";
            /* +
           "<div style='width:100%; bottom:110px; left:0; position:relative; display:block;'>" +
               "<span style='color:#fff; font-size:2.5em; font-family:georgia; margin-left:1em;'>" + (!String.IsNullOrEmpty(header)?header:"CV Document") + "</span>" +
           "</div>";*/

            return headerHtml;

        }
    }
}