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
            IAccessValidator accessValidator = new AccessValidator();
            CCFServicesManager.RegisterService(() => accessValidator);
            while (true)
            {
                Console.WriteLine("Go to wait!");
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }
    }

    class AccessValidator : IAccessValidator
    {
        public bool CanAccessToFolder(string userToken, string targetFolder) =>
            true;
    }
    public interface IAccessValidator
    {
        /// <summary>
        /// Проверяет возможность человека получить доступ к ресурсу.
        /// </summary>
        /// <param name="userToken">Токен человека, который запрашивает доступ</param>
        /// <param name="targetFolder">Путь к желаемой папке, включает в себя id персоны владелья</param>
        /// <returns></returns>
        bool CanAccessToFolder(string userToken, string targetFolder);
    }
}
