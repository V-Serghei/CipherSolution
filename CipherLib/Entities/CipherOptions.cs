namespace CipherLib.Entities
{
    public class CipherOptions(
        bool useExplicitAlphabet = false,
        string alphabetVariant = "eng",
        bool allowSymbols = false,
        bool allowNumbers = false,
        bool errorLogging = false,
        bool processLogging = false)
    {
        public bool UseExplicitAlphabet { get; set; } = useExplicitAlphabet;
        public string AlphabetVariant { get; set; } = alphabetVariant;
        public bool AllowSymbols { get; set; } = allowSymbols;
        public bool AllowNumbers { get; set; } = allowNumbers;
        public bool ErrorLogging { get; set; } = errorLogging;
        public bool ProcessLogging { get; set; } = processLogging;
    }
}