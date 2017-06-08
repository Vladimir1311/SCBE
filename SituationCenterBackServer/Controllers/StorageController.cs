using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Models.StorageModels;

namespace SituationCenterBackServer.Controllers
{
    public class StorageController : Controller
    {
        public IActionResult Index()
        {
            DirectoryContent content = new DirectoryContent
            {
                Directories = new Directory[]
                {
                    new Directory
                    {
                        Name = "Folder 1"
                    },
                    new Directory
                    {
                        Name = "Folder 2"
                    },
                    new Directory
                    {
                        Name = "Folder 3"
                    }
                },
                Files = new File[]
                {
                    new File
                    {
                        Name = "File 1"
                    },
                    new File
                    {
                        Name = "File 2"
                    },
                    new File
                    {
                        Name = "File 3"
                    }
                }
            };
            return View(content);
        }
    }
}