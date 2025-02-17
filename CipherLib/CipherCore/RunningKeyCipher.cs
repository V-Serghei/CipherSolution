using CipherLib.AbstractFactory;
using CipherLib.ConstVal;
using CipherLib.Entities;

namespace CipherLib.CipherCore
{
    public class RunningKeyCipher : ICipher
    {
        private string _runningKey;
        private char[] _alphabet;
        private bool _enableErrorLogging = false;
        private bool _enableProcessLogging = false;

        private char[] DetermineAlphabet(string text)
        {
            bool containsRussian = text.Any(c => (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'));
            return containsRussian ? Alphabet.Default.RusAlphabet : Alphabet.Default.EngAlphabet;
        }

        public RunningKeyCipher(ICipherConfiguration configuration)
            : this(configuration.Key, configuration.Options) { }
        
        public RunningKeyCipher(string runningKey,  CipherOptions? options = null)
        {
            _runningKey = runningKey;
            if (!options.UseExplicitAlphabet)
            {
                _alphabet = DetermineAlphabet(runningKey);
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
