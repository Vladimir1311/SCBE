using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCF;
using DocsToPictures.Models;
using DocsToPictures.Interfaces;
using System.Threading;
using System.IO;

namespace DocsToPictures
{
    class Program
    {
        static void Main(string[] args)
        {
            IDocumentProcessor processor = new DocumentProcessor();
            CCFServicesManager.RegisterService(() =>processor);
            while (true)
            {
                Console.WriteLine("Go to wait!");
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }
    }
}
