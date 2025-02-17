using CipherLib.Entities;

namespace CipherLib.AbstractFactory.SpecificConfigurations;

public class AutoKeyCipherConfiguration: ICipherConfiguration
{
    public string Key { get; }
    public CipherOptions Options { get; }

    public AutoKeyCipherConfiguration(string key, CipherOptions options)
    {
        Key = key;
        Options = options;
    }
    
}