using CipherLib.AbstractFactory;
using CipherLib.ConstVal;
using CipherLib.Entities;
using CipherLib.Infrastructure;

namespace CipherLib.CipherCore
{
    public class VigenereCipher : ICipher
    {
        private string _key;
        private char[] _alphabet;
        private bool _enableErrorLogging = false;
        private bool _enableProcessLogging = false;
        
        WayManager manager = new WayManager();
        private char[] DetermineAlphabet(string text)
        {
            bool containsRussian = text.Any(c => (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'));
            return containsRussian ? Alphabet.Default.RusAlphabet : Alphabet.Default.EngAlphabet;
        }

        public VigenereCipher(ICipherConfiguration configuration)
            : this(configuration.Key, configuration.Options) { }
        
        public VigenereCipher(string key, CipherOptions? options = null)
        {
            _key = key;
            if (options == null )
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
            return Process(text, _key, true);
        }

        public string Decrypt(string text)
        {
            _alphabet = DetermineAlphabet(text);
            return Process(text, _key, false);
        }

        // private string Process(string text, string key, bool encrypt)
        // {
        //     string result = "";
        //     int alphabetLength = _alphabet.Length;
        //     
        //     for (int i = 0, j = 0; i < text.Length; i++)
        //     {
        //         char textChar = text[i];
        //         int textIndex = Array.IndexOf(_alphabet, textChar);
        //         if (textIndex == -1)
        //         {
        //             result += textChar;
        //             continue;
        //         }
        //         
        //         char keyChar = key[j % key.Length];
        //         int keyIndex = Array.IndexOf(_alphabet, keyChar);
        //         if (keyIndex == -1)
        //             keyIndex = 0;
        //
        //         int newIndex = encrypt
        //             ? (textIndex + keyIndex) % alphabetLength
        //             : (textIndex - keyIndex + alphabetLength) % alphabetLength;
        //
        //         result += _alphabet[newIndex];
        //         j++;
        //     }
        //
        //     return result;
        // }
        //
        private string Process(string text, string key, bool encrypt)
        {
            List<int> way = new List<int>();
            way.Add(1);
            string result = "";
            int alphabetLength = _alphabet.Length;
            way.Add(2);
            for (int i = 0, j = 0; i < text.Length; i++)
            {
                
                way.Add(3);
                char textChar = text[i];
                int textIndex = Array.IndexOf(_alphabet, textChar);
                way.Add(4);
                if (textIndex != -1)
                {
                    way.Add(5);
                    char keyChar = key[j % key.Length];
                    int keyIndex = Array.IndexOf(_alphabet, keyChar);
                    way.Add(6);
                    if (keyIndex == -1)
                    {
                        way.Add(7);
                        keyIndex = 0;
                    }
                    way.Add(8);
                    
                    way.Add(9);
                    int newIndex = encrypt
                        ? (textIndex + keyIndex) % alphabetLength
                        : (textIndex - keyIndex + alphabetLength) % alphabetLength;
                    if(encrypt)way.Add(10);
                    else way.Add(11);
                    way.Add(12);
                    
                    result += _alphabet[newIndex];
                    j++;
                }
                else
                {
                    way.Add(13);
                    result += textChar;
                }
                way.Add(14);
                way.Add(15);
                way.Add(2);
            }
            way.Add(16);
            
            outputWay(way);
            return result;
        }

        private void outputWay(List<int> way)
        {
            manager.OutputWay(way);
            foreach (var CWay in way)
            {
                Console.Write(CWay);
                if (!CWay.Equals(way[^1])) 
                {
                    Console.Write("->");
                }
            }
            Console.WriteLine();
            
            
        }


        public void SetKey(string key)
        {
            _key = key;
        }
    }
}
