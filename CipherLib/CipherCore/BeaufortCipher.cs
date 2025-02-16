using CipherLib.ConstVal;
using CipherLib.Entities;

namespace CipherLib.CipherCore
{
    public class BeaufortCipher : ICipher
    {
        
        private string _key;
        private char[] _alphabet;
        private bool _enableErrorLogging = false;
        private bool _enableProcessLogging = false;
        private char[] DetermineAlphabet(string text)
        {
            bool containsRussian = text.Any(c => (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'));
            return containsRussian ? Alphabet.Default.RusAlphabet : Alphabet.Default.EngAlphabet;
        }

        public BeaufortCipher(string key, CipherOptions? options = null)
        {
            _key = key;
            if (options is { UseExplicitAlphabet: false })
            {
                _alphabet = DetermineAlphabet(key);
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

        public string Encrypt(string text)
        {
            _alphabet = DetermineAlphabet(text);
            return Process(text, _key);
        }

        public string Decrypt(string text)
        {
            _alphabet = DetermineAlphabet(text);
            return Process(text, _key);
        }

        private string Process(string text, string key)
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

                int newIndex = (keyIndex - textIndex + alphabetLength) % alphabetLength;

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
