using CipherLib.AbstractFactory.SpecificConfigurations;
using CipherLib.CipherCore;
using CipherLib.Entities;
using Logging;

namespace CipherLib.AbstractFactory.AbstractFactoryConcrete;

public class BeaufortCipherComponentsFactory:ICipherComponentsFactory
{
    public ICipherConfiguration CreateConfiguration(string key, CipherOptions options)
    {
        return new BeaufortCipherConfiguration(key, options);
    }

    public ICipher CreateCipher(ICipherConfiguration configuration)
    {
        return new BeaufortCipher(configuration);
    }

    public ILogger CreateProcessLogger()
    {
        return ProcessLoggerLazy.Instance;
    }
}