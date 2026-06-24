using CipherLib.ConstVal;
using CipherLib.Entities;

namespace CipherLib.CipherCore
{
    public abstract class CipherBase : ICipher
    {
        protected string _key;
        protected char[] _alphabet;
        protected bool _enableErrorLogging;
        protected bool _enableProcessLogging;
        private readonly bool _useExplicitAlphabet;

        protected CipherBase(string key, CipherOptions? options)
        {
            _key = key;
            if (options == null || !options.UseExplicitAlphabet)
            {
                _useExplicitAlphabet = false;
                _alphabet = DetectAlphabet(key);
            }
            else
            {
                _useExplicitAlphabet = true;
                _alphabet = ResolveAlphabet(options);
                _enableErrorLogging = options.ErrorLogging;
                _enableProcessLogging = options.ProcessLogging;
            }
        }

        // Returns the alphabet appropriate for a given text, respecting the explicit-alphabet setting.
        protected char[] GetAlphabetForText(string text) =>
            _useExplicitAlphabet ? _alphabet : DetectAlphabet(text);

        private static char[] DetectAlphabet(string text)
        {
            bool containsRussian = text.Any(c => (c >= 'А' && c <= 'я') || c == 'Ё' || c == 'ё');
            return containsRussian ? Alphabet.Default.RusAlphabet : Alphabet.Default.EngAlphabet;
        }

        private static char[] ResolveAlphabet(CipherOptions options) =>
            options.AlphabetVariant.ToLower() switch
            {
                "rus"          => Alphabet.Default.RusAlphabet,
                "eng"          => Alphabet.Default.EngAlphabet,
                "rus+eng"      => Alphabet.Default.RusEngAlphabet,
                "rus+num"      => options.AllowNumbers
                                    ? Alphabet.Default.RusAlphabetWithNumbers
                                    : throw new ArgumentException("Numbers are not allowed in this variant."),
                "eng+num"      => options.AllowNumbers
                                    ? Alphabet.Default.EngAlphabetWithNumbers
                                    : throw new ArgumentException("Numbers are not allowed in this variant."),
                "rus+sym"      => options.AllowSymbols
                                    ? Alphabet.Default.RusAlphabetWithSymbols
                                    : throw new ArgumentException("Symbols are not allowed in this variant."),
                "eng+sym"      => options.AllowSymbols
                                    ? Alphabet.Default.EngAlphabetWithSymbols
                                    : throw new ArgumentException("Symbols are not allowed in this variant."),
                "rus+num+sym"  => options is { AllowSymbols: true, AllowNumbers: true }
                                    ? Alphabet.Default.RusAlphabetWithNumbersAndSymbols
                                    : throw new ArgumentException("Symbols and Numbers are not allowed in this variant."),
                "eng+num+sym"  => options is { AllowSymbols: true, AllowNumbers: true }
                                    ? Alphabet.Default.EngAlphabetWithNumbersAndSymbols
                                    : throw new ArgumentException("Symbols and Numbers are not allowed in this variant."),
                "rus+eng+num+sym" => options is { AllowSymbols: true, AllowNumbers: true }
                                    ? Alphabet.Default.EngRusAlphabetWithNumbersAndSymbols
                                    : throw new ArgumentException("Symbols and Numbers are not allowed in this variant."),
                _              => Alphabet.Default.EngAlphabet
            };

        public abstract string Encrypt(string text);
        public abstract string Decrypt(string text);
        public void SetKey(string key) => _key = key;
    }
}
