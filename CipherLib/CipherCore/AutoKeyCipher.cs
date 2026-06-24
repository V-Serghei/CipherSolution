using CipherLib.AbstractFactory;
using CipherLib.Entities;

namespace CipherLib.CipherCore
{
    public class AutoKeyCipher : CipherBase
    {
        public AutoKeyCipher(ICipherConfiguration configuration)
            : this(configuration.Key, configuration.Options) { }

        public AutoKeyCipher(string secretKey, CipherOptions? options = null) : base(secretKey, options) { }

        public override string Encrypt(string plaintext)
        {
            _alphabet = GetAlphabetForText(plaintext);
            int alphabetLength = _alphabet.Length;
            var result = new System.Text.StringBuilder(plaintext.Length);

            for (int i = 0; i < plaintext.Length; i++)
            {
                int pIndex = Array.IndexOf(_alphabet, plaintext[i]);
                if (pIndex == -1)
                {
                    result.Append(plaintext[i]);
                    continue;
                }

                // After secret key is exhausted, extend with plaintext characters
                char keyChar = i < _key.Length ? _key[i] : plaintext[i - _key.Length];
                int keyIndex = Array.IndexOf(_alphabet, keyChar);
                if (keyIndex == -1) keyIndex = 0;

                result.Append(_alphabet[(pIndex + keyIndex) % alphabetLength]);
            }

            return result.ToString();
        }

        public override string Decrypt(string ciphertext)
        {
            _alphabet = GetAlphabetForText(ciphertext);
            int alphabetLength = _alphabet.Length;
            var plaintext = new System.Text.StringBuilder(ciphertext.Length);

            for (int i = 0; i < ciphertext.Length; i++)
            {
                int cIndex = Array.IndexOf(_alphabet, ciphertext[i]);
                if (cIndex == -1)
                {
                    plaintext.Append(ciphertext[i]);
                    continue;
                }

                // After secret key is exhausted, extend with already-decrypted plaintext
                char keyChar = i < _key.Length ? _key[i] : plaintext[i - _key.Length];
                int keyIndex = Array.IndexOf(_alphabet, keyChar);
                if (keyIndex == -1) keyIndex = 0;

                plaintext.Append(_alphabet[(cIndex - keyIndex + alphabetLength) % alphabetLength]);
            }

            return plaintext.ToString();
        }
    }
}
