using CipherLib.Factory.Creators;

namespace CipherLib.Factory
{
    public static class CipherFactory
    {
        public static CipherCreator GetCipherCreator(string choice)
        {
            switch (choice)
            {
                case "1":
                case "vigenere":
                    return new VigenereCipherCreator();
                case "2":
                case "beaufort":
                    return new BeaufortCipherCreator();
                case "3":
                case "autokey":
                    return new AutoKeyCipherCreator();
                case "4":
                case "runningkey":
                    return new RunningKeyCipherCreator();
                default:
                    throw new ArgumentException("Unknown cipher type");
            }
        }
    }
}