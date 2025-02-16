using CipherLib.ConstVal;
using CipherLib.Entities;

namespace CipherLib.CipherCore
{
    public class AutoKeyCipher : ICipher
    {
        private string _secretKey;
        private char[] _alphabet;
        private bool _enableErrorLogging = false;
        private bool _enableProcessLogging = false;
        
        private char[] DetermineAlphabet(string text)
        {
            bool containsRussian = text.Any(c => (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'));
            return containsRussian ? Alphabet.Default.RusAlphabet : Alphabet.Default.EngAlphabet;
        }

        public AutoKeyCipher(string secretKey,  CipherOptions? options = null)
        {
            _secretKey = secretKey;
            if (!options.UseExplicitAlphabet)
            {
                _alphabet = DetermineAlphabet(_secretKey);
            }
            else
            {
                switch (options.AlphabetVariant.ToLower())
                {
                    case "rus":
                        _alphabet = Alphabet.Default.RusAlphabet;
                        break;
                    case "eng":
                        _alphabet = Alphabet.Default.EngAlphabet;
                        break;
                    case "rus+eng":
                        _alphabet = Alphabet.Default.RusEngAlphabet;
                        break;
                    case "rus+num":
                        if (options.AllowNumbers)
                        {
                            _alphabet = Alphabet.Default.RusAlphabetWithNumbers;
                        }
                        else
                        {
                            throw new ArgumentException("Numbers are not allowed in this variant.");
                        }
                        break;
                    case "eng+num":
                        if (options.AllowNumbers)
                        {
                            _alphabet = Alphabet.Default.EngAlphabetWithNumbers;
                        }
                        else
                        {
                            throw new ArgumentException("Numbers are not allowed in this variant.");
                        }
                        
                        break;
                    case "rus+sym":
                        if (options.AllowSymbols)
                        {
                            _alphabet = Alphabet.Default.RusAlphabetWithSymbols;
                        }
                        else
                        {
                            throw new ArgumentException("Symbols are not allowed in this variant.");
                        }
                        break;
                    case "eng+sym":
                        if (options.AllowSymbols)
                        {
                            _alphabet = Alphabet.Default.EngAlphabetWithSymbols;
                        }
                        else
                        {
                            throw new ArgumentException("Symbols are not allowed in this variant.");
                        }
                        break;
                    case "rus+num+sym":
                        if (options is { AllowSymbols: true, AllowNumbers: true })
                        {
                            _alphabet = Alphabet.Default.RusAlphabetWithNumbersAndSymbols;
                        }
                        else
                        {
                            throw new ArgumentException("Symbols and Numbers are not allowed in this variant.");
                        }
                        break;
                    case "eng+num+sym":
                        if (options is { AllowSymbols: true, AllowNumbers: true })
                        {
                            _alphabet = Alphabet.Default.EngAlphabetWithNumbersAndSymbols;
                        }
                        else
                        {
                            throw new ArgumentException("Symbols and Numbers are not allowed in this variant.");
                        }
                        break;
                    case "rus+eng+num+sym":
                        if (options is { AllowSymbols: true, AllowNumbers: true })
                        {
                            _alphabet = Alphabet.Default.EngRusAlphabetWithNumbersAndSymbols;
                        }
                        else
                        {
                            throw new ArgumentException("Symbols and Numbers are not allowed in this variant.");
                        }
                        
                        break;
                    default:
                        _alphabet = Alphabet.Default.EngAlphabet;
                        break;
                }

                _enableErrorLogging = options.ErrorLogging;
                _enableProcessLogging = options.ProcessLogging;
            }
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
