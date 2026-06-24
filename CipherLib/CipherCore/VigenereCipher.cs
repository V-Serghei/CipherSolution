using CipherLib.AbstractFactory;
using CipherLib.Entities;

namespace CipherLib.CipherCore
{
    public class VigenereCipher : CipherBase
    {
        public VigenereCipher(ICipherConfiguration configuration)
            : this(configuration.Key, configuration.Options) { }

        public VigenereCipher(string key, CipherOptions? options = null) : base(key, options) { }

        public override string Encrypt(string text)
        {
            _alphabet = GetAlphabetForText(text);
            return Process(text, encrypt: true);
        }

        public override string Decrypt(string text)
        {
            _alphabet = GetAlphabetForText(text);
            return Process(text, encrypt: false);
        }

        private string Process(string text, bool encrypt)
        {
            int alphabetLength = _alphabet.Length;
            var result = new System.Text.StringBuilder(text.Length);

            for (int i = 0, j = 0; i < text.Length; i++)
            {
                int textIndex = Array.IndexOf(_alphabet, text[i]);
                if (textIndex == -1)
                {
                    result.Append(text[i]);
                    continue;
                }

                int keyIndex = Array.IndexOf(_alphabet, _key[j % _key.Length]);
                if (keyIndex == -1) keyIndex = 0;

                int newIndex = encrypt
                    ? (textIndex + keyIndex) % alphabetLength
                    : (textIndex - keyIndex + alphabetLength) % alphabetLength;

                result.Append(_alphabet[newIndex]);
                j++;
            }

            return result.ToString();
        }
    }
}
