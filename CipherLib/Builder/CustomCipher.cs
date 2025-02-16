using CipherLib;

namespace CipherLib.Builder
{
    public class CustomCipher : ICipher
    {
        private readonly ICipher _underlyingCipher;
        private readonly string _language;
        private readonly bool _errorLogging;
        private readonly bool _processLogging;

        public CustomCipher(ICipher underlyingCipher, string language, bool errorLogging, bool processLogging)
        {
            _underlyingCipher = underlyingCipher;
            _language = language;
            _errorLogging = errorLogging;
            _processLogging = processLogging;
        }

        public string Encrypt(string text)
        {
            return _underlyingCipher.Encrypt(text);
        }

        public string Decrypt(string text)
        {
            return _underlyingCipher.Decrypt(text);
        }

        public void SetKey(string key)
        {
            _underlyingCipher.SetKey(key);
        }
    }
}