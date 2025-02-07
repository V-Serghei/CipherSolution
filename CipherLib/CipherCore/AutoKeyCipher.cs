using CipherLib.ConstVal;

namespace CipherLib.CipherCore
{
    public class AutoKeyCipher : ICipher
    {
        private string _secretKey;
        private char[] _alphabet;
        
        private char[] DetermineAlphabet(string text)
        {
            bool containsRussian = text.Any(c => (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'));
            return containsRussian ? Alphabet.Default.RusAlphabet : Alphabet.Default.EngAlphabet;
        }

        public AutoKeyCipher(string secretKey)
        {
            _secretKey = secretKey;
            
            _alphabet = DetermineAlphabet(secretKey);
        }

        
        public string Encrypt(string plaintext)
        {
            
            _alphabet = DetermineAlphabet(plaintext);
            int alphabetLength = _alphabet.Length;
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

                char keyChar;
                if (i < _secretKey.Length)
                {
                    keyChar = _secretKey[i];
                }
                else
                {
                    
                    keyChar = plaintext[i - _secretKey.Length];
                }

                int keyIndex = Array.IndexOf(_alphabet, keyChar);
                if (keyIndex == -1)
                    keyIndex = 0;

                // C = (P + key) mod alphabetLength
                int cIndex = (pIndex + keyIndex) % alphabetLength;
                ciphertext += _alphabet[cIndex];
            }
            return ciphertext;
        }

        public string Decrypt(string ciphertext)
        {
            
            _alphabet = DetermineAlphabet(ciphertext);
            int alphabetLength = _alphabet.Length;
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

                char keyChar;
                if (i < _secretKey.Length)
                {
                    keyChar = _secretKey[i];
                }
                else
                {
                    keyChar = plaintext[i - _secretKey.Length];
                }

                int keyIndex = Array.IndexOf(_alphabet, keyChar);
                if (keyIndex == -1)
                    keyIndex = 0;

                
                // P = (C - key + alphabetLength) mod alphabetLength
                int pIndex = (cIndex - keyIndex + alphabetLength) % alphabetLength;
                plaintext += _alphabet[pIndex];
            }
            return plaintext;
        }
        

        
        public void SetKey(string key)
        {
            _secretKey = key;
        }
    }
}
