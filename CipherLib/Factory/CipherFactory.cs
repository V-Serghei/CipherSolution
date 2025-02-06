using CipherLib.CipherCore;

namespace CipherLib.Factory
{
    public static class CipherFactory
    {
        public static ICipher CreateCipher(string type, string key)
        {
            switch (type.ToLower())
            {
                case "vigenere":
                    return new VigenereCipher(key);
                
                //TODO: ADD OTHER CIPHERS
                
                default:
                    throw new ArgumentException("Unknown cipher type");
            }
        }
    }
}