﻿using System.Web;
using System.Web.Optimization;

namespace ResourceApplicationTool
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
            "~/Scripts/common.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-datepicker").Include(
                      "~/Scripts/bootstrap-datepicker.min.js"));

            bundles.Add(new ScriptBundle("~/Content/bootstrap-sliderjs").Include(
          "~/Scripts/bootstrap-sliders/bootstrap-slider.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/slick").Include(
          "~/Scripts/jquery-migrate-1.2.1.js",
          "~/Scripts/slick/slick/slick.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-uijs").Include(
          "~/Scripts/jquery-ui/jquery-ui.js"));

            //css bundles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/commonStyle.css",
                      "~/Content/Styles/masterpageStyle.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap-slidercss").Include(
                      "~/Scripts/bootstrap-sliders/css/bootstrap-slider.css",
                      "~/Scripts/bootstrap-sliders/css/bootstrap-slider.min.css"));

            bundles.Add(new StyleBundle("~/Content/slickcss").Include(
                      "~/Scripts/slick/slick/slick.css",
                      "~/Scripts/slick/slick/slick-theme.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap-datepickercss").Include(
                      "~/Content/bootstrap-datepicker3.css"));

            bundles.Add(new StyleBundle("~/Content/jquery-uicss").Include(
          "~/Scripts/jquery-ui/css/jquery-ui.min.css",
          "~/Scripts/jquery-ui/css/jquery-ui.structure.css",
          "~/Scripts/jquery-ui/css/jquery-ui.theme.css"));
        }
    }
}
