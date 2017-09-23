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