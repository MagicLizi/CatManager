using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CatManager
{
    internal class Program
    {
        public static void Main(string[] args)
        {
           CommandAdapter.GetInstance().ConnectToCommandServer();
           CatAdapter.GetInstance().ConnectToCat();
           Console.ReadLine();
        }
    }
}