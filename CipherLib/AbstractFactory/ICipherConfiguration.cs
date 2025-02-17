using CipherLib.Entities;

namespace CipherLib.AbstractFactory
{
    
    public interface ICipherConfiguration
    {
        string Key { get; }
        CipherOptions Options { get; }
    }
}