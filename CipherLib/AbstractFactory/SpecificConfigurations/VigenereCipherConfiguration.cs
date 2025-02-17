using CipherLib.Entities;

namespace CipherLib.AbstractFactory.SpecificConfigurations
{
    public class VigenereCipherConfiguration : ICipherConfiguration
    {
        public string Key { get; }
        public CipherOptions Options { get; }

        public VigenereCipherConfiguration(string key, CipherOptions options)
        {
            Key = key;
            Options = options;
        }
    }
}