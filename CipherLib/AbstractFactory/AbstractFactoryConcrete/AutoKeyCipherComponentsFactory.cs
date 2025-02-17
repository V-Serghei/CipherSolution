using CipherLib.AbstractFactory.SpecificConfigurations;
using CipherLib.CipherCore;
using CipherLib.Entities;
using Logging;

namespace CipherLib.AbstractFactory.AbstractFactoryConcrete;

public class AutoKeyCipherComponentsFactory: ICipherComponentsFactory
{
    public ICipherConfiguration CreateConfiguration(string key, CipherOptions options)
    {
        return new AutoKeyCipherConfiguration(key, options);
    }

    public ICipher CreateCipher(ICipherConfiguration configuration)
    {
        return new AutoKeyCipher(configuration);
    }

    public ILogger CreateProcessLogger()
    {
        return ProcessLoggerLazy.Instance;
    }
}