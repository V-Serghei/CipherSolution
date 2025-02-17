using CipherLib.AbstractFactory.SpecificConfigurations;
using CipherLib.CipherCore;
using CipherLib.Entities;
using Logging;

namespace CipherLib.AbstractFactory.AbstractFactoryConcrete
{
    public class VigenereCipherComponentsFactory : ICipherComponentsFactory
    {
        public ICipherConfiguration CreateConfiguration(string key, CipherOptions options)
        {
            return new VigenereCipherConfiguration(key, options);
        }

        public ICipher CreateCipher(ICipherConfiguration configuration)
        {
            return new VigenereCipher(configuration);
        }

        public ILogger CreateProcessLogger()
        {
            return ProcessLoggerLazy.Instance;
        }
    }
}