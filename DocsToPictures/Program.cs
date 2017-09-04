using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCF;
using DocsToPictures.Models;
using DocsToPictures.Interfaces;
using System.Threading;

namespace DocsToPictures
{
    class Program
    {
        static void Main(string[] args)
        {
            CCFServicesManager.RegisterService(new DocumentProcessor() as IDocumentProcessor);
            while(true)
            {
                Console.WriteLine("Go to wait!");
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }
    }
}
