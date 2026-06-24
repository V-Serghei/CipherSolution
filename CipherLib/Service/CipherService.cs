namespace CipherLib.Service
{
    public class CipherService
    {
        private readonly ICipher _cipher;

        public CipherService(ICipher cipher)
        {
            _cipher = cipher ?? throw new ArgumentNullException(nameof(cipher));
        }

        public string EncryptText(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text cannot be empty.", nameof(text));
            return _cipher.Encrypt(text);
        }

        public string DecryptText(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text cannot be empty.", nameof(text));
            return _cipher.Decrypt(text);
        }

        public void SetKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be empty.", nameof(key));
            _cipher.SetKey(key);
        }
    }
}
