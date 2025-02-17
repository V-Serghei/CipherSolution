namespace CipherLib.Entities
{
    public class CipherOptions
    {
        public bool UseExplicitAlphabet { get; set; }
        public string AlphabetVariant { get; set; }
        public bool AllowSymbols { get; set; }
        public bool AllowNumbers { get; set; }
        public bool ErrorLogging { get; set; }
        public bool ProcessLogging { get; set; }

        public CipherOptions(
            bool useExplicitAlphabet = false,
            string alphabetVariant = "eng",
            bool allowSymbols = false,
            bool allowNumbers = false,
            bool errorLogging = false,
            bool processLogging = false)
        {
            UseExplicitAlphabet = useExplicitAlphabet;
            AlphabetVariant = alphabetVariant;
            AllowSymbols = allowSymbols;
            AllowNumbers = allowNumbers;
            ErrorLogging = errorLogging;
            ProcessLogging = processLogging;
        }
    }
}