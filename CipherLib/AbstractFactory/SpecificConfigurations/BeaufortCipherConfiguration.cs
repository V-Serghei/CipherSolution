using CipherLib.Entities;

namespace CipherLib.AbstractFactory.SpecificConfigurations;

public class BeaufortCipherConfiguration:ICipherConfiguration
{
    public string Key { get; }
    public CipherOptions Options { get; }

    public BeaufortCipherConfiguration(string key, CipherOptions options)
    {
        Key = key;
        Options = options;
    }
}