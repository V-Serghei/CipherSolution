using ApplicationL.CustomExceptions;
using CipherLib;
using CipherLib.Builder;
using CipherLib.ConstVal;
using Logging;

namespace ApplicationL;

public class BuilderModeUse
{
    private static readonly ProcessLogger _logger = ProcessLogger.Instance;
    private static readonly ILogger _errorLogger = ErrorLogger.Instance;

    public void Run()
    {
        bool restart;
        do
        {
            restart = false;
            ICipher? cipher = BuildCipher();

            if (cipher == null)
            {
                Console.WriteLine("Failed to build cipher. Try again.");
                restart = true;
                continue;
            }

            while (true)
            {
                Console.WriteLine("\nSelect an action:");
                Console.WriteLine("1. Encrypt text");
                Console.WriteLine("2. Decrypt text");
                Console.WriteLine("3. Change key");
                Console.WriteLine("00. Choose a different cipher");
                Console.WriteLine("0. Exit");

                string? input = Console.ReadLine();

                if (input == "0")
                {
                    _logger.LogD("Exit");
                    break;
                }

                if (input == "00")
                {
                    _logger.LogD("Cipher change requested");
                    restart = true;
                    break;
                }

                try
                {
                    switch (input)
                    {
                        case "1":
                        {
                            Console.Write("Enter text to encrypt: ");
                            string text = Console.ReadLine() ?? "";
                            if (string.IsNullOrEmpty(text))
                                throw new InvalidTextException("Text cannot be empty.");
                            Console.WriteLine($"Encrypted: {cipher.Encrypt(text)}");
                            _logger.LogD("Encrypted", text);
                            break;
                        }
                        case "2":
                        {
                            Console.Write("Enter text to decrypt: ");
                            string text = Console.ReadLine() ?? "";
                            if (string.IsNullOrEmpty(text))
                                throw new InvalidTextException("Text cannot be empty.");
                            Console.WriteLine($"Decrypted: {cipher.Decrypt(text)}");
                            _logger.LogD("Decrypted", text);
                            break;
                        }
                        case "3":
                        {
                            Console.Write("Enter new key: ");
                            string key = Console.ReadLine() ?? "";
                            if (string.IsNullOrEmpty(key))
                                throw new InvalidKeyException("Key cannot be empty.");
                            cipher.SetKey(key);
                            _logger.LogD("Key changed", key);
                            break;
                        }
                        default:
                            Console.WriteLine("Invalid command.");
                            break;
                    }
                }
                catch (InvalidTextException ex)
                {
                    Console.WriteLine($"Invalid text: {ex.Message}");
                    _errorLogger.LogD(ex.Message, ex);
                }
                catch (InvalidKeyException ex)
                {
                    Console.WriteLine($"Invalid key: {ex.Message}");
                    _errorLogger.LogD(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    _errorLogger.LogD(ex.Message, ex);
                }
            }

        } while (restart);
    }

    private static ICipher? BuildCipher()
    {
        Console.WriteLine("\nSelect builder mode:");
        Console.WriteLine("1. Standard configuration (via Director)");
        Console.WriteLine("2. Manual configuration");
        string? builderMode = Console.ReadLine();

        ICipherBuilder builder = new CipherBuilder();
        CipherDirector director = new CipherDirector();

        if (builderMode == "1")
        {
            Console.WriteLine("Select algorithm:");
            Console.WriteLine("1. Vigenère  2. Beaufort  3. Autokey  4. Running key");
            string? algChoice = Console.ReadLine();

            Console.Write("Enter key: ");
            string key = Console.ReadLine() ?? "Default";

            switch (algChoice)
            {
                case "1": director.BuildDefaultVigenere(builder, key); break;
                case "2": director.BuildDefaultBeaufort(builder, key); break;
                case "3": director.BuildDefaultAutoKey(builder, key); break;
                case "4": director.BuildDefaultRunningKey(builder, key); break;
                default:
                    Console.WriteLine("Unknown choice, using default Vigenère.");
                    director.BuildDefaultVigenere(builder);
                    break;
            }
        }
        else
        {
            Console.WriteLine("Select algorithm type:");
            Console.WriteLine("1. Vigenère  2. Beaufort  3. Autokey  4. Running key");
            string? algChoice = Console.ReadLine();

            switch (algChoice)
            {
                case "1": builder.SetAlgorithmType(CipherType.Vigenere); break;
                case "2": builder.SetAlgorithmType(CipherType.Beaufort); break;
                case "3": builder.SetAlgorithmType(CipherType.AutoKey); break;
                case "4": builder.SetAlgorithmType(CipherType.RunningKey); break;
                default:
                    Console.WriteLine("Unknown choice, using Vigenère.");
                    builder.SetAlgorithmType(CipherType.Vigenere);
                    break;
            }

            Console.Write("Key: ");
            builder.SetKey(Console.ReadLine()!);
            Console.Write("Salt (leave empty to skip): ");
            builder.SetSalt(Console.ReadLine()!);
            Console.Write("Allow symbols? (true/false): ");
            builder.AllowSymbols(Console.ReadLine()?.ToLower() == "true");
            Console.Write("Allow numbers? (true/false): ");
            builder.AllowNumbers(Console.ReadLine()?.ToLower() == "true");
            Console.Write("Language (eng / rus / rus+eng / etc.): ");
            builder.SetLanguage(Console.ReadLine()!);
            Console.Write("Enable error logging? (true/false): ");
            builder.EnableErrorLogging(Console.ReadLine()?.ToLower() == "true");
            Console.Write("Enable process logging? (true/false): ");
            builder.EnableProcessLogging(Console.ReadLine()?.ToLower() == "true");
        }

        try
        {
            return builder.Build();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to build cipher: {ex.Message}");
            return null;
        }
    }
}
