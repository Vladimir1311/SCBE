using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Office.Interop.Excel;

namespace DocsToPictures.Controllers
{
    public class ExcelToPictureController : Controller
    {
        public string GetText()
        {
            //Workbook workbook = new Workbook();

            //workbook.LoadFromFile(Server.MapPath("~/App_Data/Book.xlsx"));

            //save to image
            //Worksheet sheet = workbook.Worksheets[0];
            ////sheet.SaveToImage(@"..\..\sample.bmp");
            return System.IO.File.ReadAllText(Server.MapPath("~/App_Data/TextFile1.txt"));
        }
    }
}
