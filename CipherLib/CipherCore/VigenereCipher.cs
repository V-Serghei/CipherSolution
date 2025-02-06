using CipherLib.ConstVal;

namespace CipherLib.CipherCore
{
    public class VigenereCipher : ICipher
    {
        private string _key;
        private char[] _alphabet;

        private char[] DetermineAlphabet(string text)
        {
            bool containsRussian = text.Any(c => (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'));
            return containsRussian ? Alphabet.Default.RusAlphabet : Alphabet.Default.EngAlphabet;
        }

        public VigenereCipher(string key)
        {
            _key = key;
            _alphabet = DetermineAlphabet(key);
        }

        public string Encrypt(string text)
        {
            _alphabet = DetermineAlphabet(text);
            return Process(text, _key, true);
        }

        public string Decrypt(string text)
        {
            _alphabet = DetermineAlphabet(text);
            return Process(text, _key, false);
        }

        private string Process(string text, string key, bool encrypt)
        {
            string result = "";
            int alphabetLength = _alphabet.Length;
            
            for (int i = 0, j = 0; i < text.Length; i++)
            {
                char textChar = text[i];
                int textIndex = Array.IndexOf(_alphabet, textChar);
                if (textIndex == -1)
                {
                    result += textChar;
                    continue;
                }
                
                char keyChar = key[j % key.Length];
                int keyIndex = Array.IndexOf(_alphabet, keyChar);
                if (keyIndex == -1)
                    keyIndex = 0;

                int newIndex = encrypt
                    ? (textIndex + keyIndex) % alphabetLength
                    : (textIndex - keyIndex + alphabetLength) % alphabetLength;

                result += _alphabet[newIndex];
                j++;
            }

            return result;
        }

        public void SetKey(string key)
        {
            _key = key;
        }
    }
}
