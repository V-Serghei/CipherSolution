using CipherLib.ConstVal;

namespace CipherLib.CipherCore
{
    public class RunningKeyCipher : ICipher
    {
        private string _runningKey;
        private char[] _alphabet;

        private char[] DetermineAlphabet(string text)
        {
            bool containsRussian = text.Any(c => (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'));
            return containsRussian ? Alphabet.Default.RusAlphabet : Alphabet.Default.EngAlphabet;
        }

        public RunningKeyCipher(string runningKey)
        {
            _runningKey = runningKey;
            _alphabet = DetermineAlphabet(runningKey);
        }

        public string Encrypt(string plaintext)
        {
            _alphabet = DetermineAlphabet(plaintext);
            int alphabetLength = _alphabet.Length;

            if (_runningKey.Length < plaintext.Length)
                throw new ArgumentException("The runaround key must be no shorter than the encrypted text.");

            string ciphertext = "";
            for (int i = 0; i < plaintext.Length; i++)
            {
                char pChar = plaintext[i];
                int pIndex = Array.IndexOf(_alphabet, pChar);
                if (pIndex == -1)
                {
                    ciphertext += pChar;
                    continue;
                }

                char keyChar = _runningKey[i];
                int keyIndex = Array.IndexOf(_alphabet, keyChar);
                if (keyIndex == -1)
                    keyIndex = 0;

                int cIndex = (pIndex + keyIndex) % alphabetLength;
                ciphertext += _alphabet[cIndex];
            }
            return ciphertext;
        }

        public string Decrypt(string ciphertext)
        {
            _alphabet = DetermineAlphabet(ciphertext);
            int alphabetLength = _alphabet.Length;

            if (_runningKey.Length < ciphertext.Length)
                throw new ArgumentException("The running key should be no shorter than the ciphertext.");

            string plaintext = "";
            for (int i = 0; i < ciphertext.Length; i++)
            {
                char cChar = ciphertext[i];
                int cIndex = Array.IndexOf(_alphabet, cChar);
                if (cIndex == -1)
                {
                    plaintext += cChar;
                    continue;
                }

                char keyChar = _runningKey[i];
                int keyIndex = Array.IndexOf(_alphabet, keyChar);
                if (keyIndex == -1)
                    keyIndex = 0;

                int pIndex = (cIndex - keyIndex + alphabetLength) % alphabetLength;
                plaintext += _alphabet[pIndex];
            }
            return plaintext;
        }

        public void SetKey(string runningKey)
        {
            _runningKey = runningKey;
        }
    }
}
