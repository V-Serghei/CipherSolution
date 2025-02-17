using CipherLib.Entities;
using Logging;

namespace CipherLib.AbstractFactory
{
   
    public interface ICipherComponentsFactory
    {
        ICipherConfiguration CreateConfiguration(string key, CipherOptions options);

        ICipher CreateCipher(ICipherConfiguration configuration);

        ILogger CreateProcessLogger();
    }
}