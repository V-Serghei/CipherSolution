namespace CipherLib.Service
{
    public class CipherService
    {
        private readonly ICipher _cipher;
        
        public CipherService(ICipher cipher)
        {
            _cipher = cipher;
        }

        public string EncryptText(string text)
        {
            return _cipher.Encrypt(text);
        }

        public string DecryptText(string text)
        {
            return _cipher.Decrypt(text);
        }

        public void SetKey(string key)
        {
            _cipher.SetKey(key);
        }
    }
}