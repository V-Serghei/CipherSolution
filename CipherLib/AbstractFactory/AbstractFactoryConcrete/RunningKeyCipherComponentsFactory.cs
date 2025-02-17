using CipherLib.AbstractFactory.SpecificConfigurations;
using CipherLib.CipherCore;
using CipherLib.Entities;
using Logging;

namespace CipherLib.AbstractFactory.AbstractFactoryConcrete;

public class RunningKeyCipherComponentsFactory:ICipherComponentsFactory
{
    public ICipherConfiguration CreateConfiguration(string key, CipherOptions options)
    {
        return new RunningKeyCipherConfiguration(key, options);
    }

    public ICipher CreateCipher(ICipherConfiguration configuration)
    {
        return new RunningKeyCipher(configuration);
    }

    public ILogger CreateProcessLogger()
    {
        return ProcessLoggerLazy.Instance;
    }
}