using ApplicationL.CustomExceptions;
using CipherLib;
using CipherLib.Builder;
using CipherLib.ConstVal;
using CipherLib.Factory;
using CipherLib.Service;
using Logging;

namespace ApplicationL
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Select the mode:");
            Console.WriteLine("1. Use ready algorithm (factory)");
            Console.WriteLine("2. Builder mode (manually configure algorithm)");
            string? mode = Console.ReadLine();
            ICipher cipher = null!;

            if (mode == "2")
            {
                BuilderModeUse builderModeUse = new BuilderModeUse();
                builderModeUse.Run();
            
            }
            else
            {
                FactoryUse factoryUse = new FactoryUse();
                factoryUse.Run();
            }
        }
    }
}
