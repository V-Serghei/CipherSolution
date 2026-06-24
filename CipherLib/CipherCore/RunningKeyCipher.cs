using CipherLib.AbstractFactory;
using CipherLib.Entities;

namespace CipherLib.CipherCore
{
    public class RunningKeyCipher : CipherBase
    {
        public RunningKeyCipher(ICipherConfiguration configuration)
            : this(configuration.Key, configuration.Options) { }

        public RunningKeyCipher(string runningKey, CipherOptions? options = null) : base(runningKey, options) { }

        public override string Encrypt(string plaintext)
        {
            _alphabet = GetAlphabetForText(plaintext);
            if (_key.Length < plaintext.Length)
                throw new ArgumentException("The running key must be no shorter than the text to encrypt.");

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

                int keyIndex = Array.IndexOf(_alphabet, _key[i]);
                if (keyIndex == -1) keyIndex = 0;

                result.Append(_alphabet[(pIndex + keyIndex) % alphabetLength]);
            }

            return result.ToString();
        }

        public override string Decrypt(string ciphertext)
        {
            _alphabet = GetAlphabetForText(ciphertext);
            if (_key.Length < ciphertext.Length)
                throw new ArgumentException("The running key must be no shorter than the ciphertext.");

            int alphabetLength = _alphabet.Length;
            var result = new System.Text.StringBuilder(ciphertext.Length);

            for (int i = 0; i < ciphertext.Length; i++)
            {
                int cIndex = Array.IndexOf(_alphabet, ciphertext[i]);
                if (cIndex == -1)
                {
                    result.Append(ciphertext[i]);
                    continue;
                }

                int keyIndex = Array.IndexOf(_alphabet, _key[i]);
                if (keyIndex == -1) keyIndex = 0;

                result.Append(_alphabet[(cIndex - keyIndex + alphabetLength) % alphabetLength]);
            }

            return result.ToString();
        }
    }
}
