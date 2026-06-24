using System.Text.Json;
using ApplicationL.CustomExceptions;
using CipherLib;
using CipherLib.Factory;
using CipherLib.Prototype;
using CipherLib.Service;
using Logging;

namespace ApplicationL;

public class FactoryUse
{
    private static readonly ProcessLogger _logger = ProcessLogger.Instance;
    private static readonly ILogger _errorLogger = ErrorLogger.Instance;

    public void Run()
    {
        Console.WriteLine("In this mode, the alphabet is determined automatically from the input text.");
        Console.WriteLine("The key and text must use the same alphabet for correct results.");
        Console.WriteLine();
        Console.WriteLine("Available alphabets:");
        Console.WriteLine("  Russian: АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюя");
        Console.WriteLine("  English: ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
        Console.WriteLine();

        bool restart;
        do
        {
            restart = false;

            ICipher? cipher = null;
            EncryptionSessionManager? sessionManager = null;
            string key = "";

            while (cipher == null)
            {
                Console.WriteLine("Select encryption algorithm:");
                Console.WriteLine("1. Vigenère cipher");
                Console.WriteLine("2. Beaufort cipher");
                Console.WriteLine("3. Autokey cipher");
                Console.WriteLine("4. Running key cipher");
                Console.WriteLine("0. Exit");

                string? choice = Console.ReadLine();
                if (choice == "0")
                {
                    _logger.LogD("Exit");
                    return;
                }

                try
                {
                    if (string.IsNullOrEmpty(choice))
                        throw new InvalidCipherChoiceException("Invalid cipher choice");

                    CipherCreator creator = CipherFactory.GetCipherCreator(choice);
                    _logger.LogD("Cipher selected", choice);

                    Console.Write("Enter the key: ");
                    key = Console.ReadLine() ?? "";
                    if (string.IsNullOrEmpty(key))
                        throw new InvalidKeyException("Invalid key");

                    cipher = creator.CreateCipher(key);
                    sessionManager = new EncryptionSessionManager(key);
                    _logger.LogD("Cipher created", key);
                }
                catch (InvalidCipherChoiceException ex)
                {
                    Console.WriteLine("Invalid choice. Enter 0 to exit.");
                    _errorLogger.LogD(ex.Message, ex);
                }
                catch (InvalidKeyException ex)
                {
                    Console.WriteLine("Invalid key. Enter 0 to exit.");
                    _errorLogger.LogD(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    _errorLogger.LogD(ex.Message, ex);
                }
            }

            var service = new CipherService(cipher);

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
                            string encrypted = service.EncryptText(text);
                            Console.WriteLine($"Encrypted: {encrypted}");
                            sessionManager!.LogOperation(true, text, encrypted, key);
                            _logger.LogD("Encrypted", text);
                            break;
                        }
                        case "2":
                        {
                            Console.Write("Enter text to decrypt: ");
                            string text = Console.ReadLine() ?? "";
                            string decrypted = service.DecryptText(text);
                            Console.WriteLine($"Decrypted: {decrypted}");
                            sessionManager!.LogOperation(false, text, decrypted, key);
                            _logger.LogD("Decrypted", text);
                            break;
                        }
                        case "3":
                        {
                            Console.Write("Enter new key: ");
                            string newKey = Console.ReadLine() ?? "";
                            if (string.IsNullOrEmpty(newKey))
                                throw new InvalidKeyException("Invalid key");
                            key = newKey;
                            cipher.SetKey(key);
                            _logger.LogD("Key changed", key);
                            break;
                        }
                        default:
                            Console.WriteLine("Invalid command.");
                            break;
                    }
                }
                catch (InvalidKeyException ex)
                {
                    Console.WriteLine("Invalid key.");
                    _errorLogger.LogD(ex.Message, ex);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Input error: {ex.Message}");
                    _errorLogger.LogD(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    _errorLogger.LogD(ex.Message, ex);
                }
            }

            if (!restart && sessionManager != null)
            {
                SaveSessions(sessionManager);
            }

        } while (restart);
    }

    private static void SaveSessions(EncryptionSessionManager sessionManager)
    {
        try
        {
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "sessions");
            Directory.CreateDirectory(directoryPath);

            string filePath = Path.Combine(directoryPath, "allSessions.json");
            string json = JsonSerializer.Serialize(
                sessionManager.GetAllSessions(),
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(filePath, json);
            Console.WriteLine($"Session data saved to: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save session data: {ex.Message}");
        }
    }
}
