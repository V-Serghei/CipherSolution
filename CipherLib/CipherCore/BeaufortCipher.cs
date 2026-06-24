using CipherLib.AbstractFactory;
using CipherLib.Entities;

namespace CipherLib.CipherCore
{
    public class BeaufortCipher : CipherBase
    {
        public BeaufortCipher(ICipherConfiguration configuration)
            : this(configuration.Key, configuration.Options) { }

        public BeaufortCipher(string key, CipherOptions? options = null) : base(key, options) { }

        // Beaufort is self-reciprocal: Encrypt == Decrypt
        public override string Encrypt(string text)
        {
            _alphabet = GetAlphabetForText(text);
            return Process(text);
        }

        public override string Decrypt(string text)
        {
            _alphabet = GetAlphabetForText(text);
            return Process(text);
        }

        private string Process(string text)
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

                result.Append(_alphabet[(keyIndex - textIndex + alphabetLength) % alphabetLength]);
                j++;
            }

            return result.ToString();
        }
    }
}
