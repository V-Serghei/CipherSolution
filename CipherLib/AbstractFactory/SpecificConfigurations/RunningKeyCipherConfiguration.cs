using CipherLib.Entities;

namespace CipherLib.AbstractFactory.SpecificConfigurations;

public class RunningKeyCipherConfiguration: ICipherConfiguration
{
    public string Key { get; }
    public CipherOptions Options { get; }

    public RunningKeyCipherConfiguration(string key, CipherOptions options)
    {
        Key = key;
        Options = options;
    }
}