using System;
using System.Collections.Generic;
using System.Text;

namespace UDPTester
{
    public class CustomActions
    {
        public virtual void Some(string value)
        {
            Console.WriteLine($"Hi! I write a {value}");
        }
    }
}
