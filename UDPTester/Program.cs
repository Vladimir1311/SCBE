using CCF;
using System;

namespace UDPTester
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Func<ILOL> lolcreater = () => new lol();
            CCFServicesManager.RegisterService(lolcreater);
            Console.WriteLine("i register new service!! Yeah!!");
            var loler = CCFServicesManager.GetService<ILOL>();
            Console.WriteLine("Yey! I recieve service ILOL!");
            Console.WriteLine($"string length from iloler is {loler.StrLength("12345")}, must be 5 ^)");
            Console.WriteLine("Happy end!)");
        }
    }

    class lol : ILOL
    {
        public int StrLength(string str) => str.Length;
    }
    public interface ILOL
    {
        int StrLength(string str);
    }
}