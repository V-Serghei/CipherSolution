using System.Text.Json;
using ApplicationL.CustomExceptions;
using CipherLib;
using CipherLib.Factory;
using CipherLib.Prototype;
using CipherLib.Service;
using Logging;

namespace ApplicationL;

public class FactoryUse()
{
    private static readonly ProcessLogger logger = ProcessLogger.Instance;
    private static readonly ILogger errorLogger = ErrorLogger.Instance;
    private static string _key = "";
    private ICipher _cipher  = null!;
    EncryptionSessionManager _sessionManager = null!;

    public void Run()
    {
        Console.WriteLine("In this mode, the alphabet will not be changed after the key is entered.");
        Console.WriteLine("The language will be chosen when entering the key from the two available options.");
        Console.WriteLine("The key and input text must be of the same alphabet.");
        Console.WriteLine("If this rule is not followed, the encryption process will not work correctly.");
        Console.WriteLine();
        Console.WriteLine("Currently available alphabets:\n" +
                          "Rus 1: АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюя \n" +
                          "Eng 2: ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz \n");
        
        // choiceCipher - is goto label for switch cipher
        choiceCipher:

        do
        {
            Console.WriteLine("Select encryption algorithm:");
            Console.WriteLine("1. Vigenère cipher");
            Console.WriteLine("2. Beaufort cipher");
            Console.WriteLine("3. Autokey cipher");
            Console.WriteLine("4. Running key cipher");
            string? choice = Console.ReadLine();
            if (choice == "0")
            {
                logger.LogD("Exit");
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(choice))
                {
                    throw new InvalidCipherChoiceException("Invalid cipher choice");
                }

                CipherCreator creator = CipherFactory.GetCipherCreator(choice);
                logger.LogD("Cipher created", choice);
                Console.WriteLine("Enter the key:");
                _key = Console.ReadLine();
                if (string.IsNullOrEmpty(_key))
                {
                    throw new InvalidKeyException("Invalid key");
                }
                _sessionManager = new EncryptionSessionManager(_key);
                logger.LogD("Cipher created", _key);
                _cipher = creator.CreateCipher(_key);
                break;

            }
            catch (InvalidCipherChoiceException exception)
            {
                Console.WriteLine("!!!!Wrong choice!!!!");
                Console.WriteLine("If you want to exit, enter 0.");
                errorLogger.LogD(exception.Message, exception);
            }
            catch (InvalidKeyException exception)
            {
                Console.WriteLine("!!!!Wrong key!!!!");
                Console.WriteLine("If you want to exit, enter 0.");
                Console.WriteLine(exception);
                errorLogger.LogD(exception.Message, exception);

            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred:");
                Console.WriteLine(ex);
                errorLogger.LogD(ex.Message, ex);
            }


        } while (true);

        var service = new CipherService(_cipher);

        while (true)
        {
            Console.WriteLine("\nSelect an action:");
            Console.WriteLine("1. Encrypt text");
            Console.WriteLine("2. Decipher the text");
            Console.WriteLine("3. Change key");
            Console.WriteLine("0. Exit");
            Console.WriteLine("00. Choose cipher");

            var input = Console.ReadLine();
            string? text = null;
            if (input == "0")
            {
                logger.LogD("Exit");
                break;
            }

            try
            {

                switch (input)
                {
                    case "1":
                    {
                        Console.Write("Enter the text: ");
                        break;
                    }
                    case "2":
                    {
                        Console.Write("Enter encrypted text: ");
                        break;
                    }
                    case "3":
                        Console.Write("Enter new key: ");
                        _key = Console.ReadLine();
                        if (string.IsNullOrEmpty(_key))
                            throw new InvalidKeyException("Invalid key");
                        _cipher.SetKey(_key);
                        logger.LogD("Key changed", _key);
                        break;
                    case "00":
                        logger.LogD("Cipher changed");
                        goto choiceCipher;
                    default:
                        Console.WriteLine("!!!!!!!Invalid command!!!!!!!!!");
                        break;
                }


                if (input != "3" && !string.IsNullOrEmpty(input) && input is "1" or "2")
                {
                    text = Console.ReadLine();
                    // if (string.IsNullOrEmpty(text))
                    // {
                    //     throw new InvalidTextException("Invalid text");
                    // }

                    switch (input)
                    {
                        case "1":
                        {
                            var encrypted = service.EncryptText(text);
                            Console.WriteLine($"Encrypted text: {encrypted}");
                            _sessionManager.LogOperation(true, text, encrypted, _key);
                            logger.LogD("Text encrypted", text);
                            break;
                        }
                        case "2":
                        {
                            var decrypted = service.DecryptText(text);
                            Console.WriteLine($"Decrypted text: {decrypted}");
                            _sessionManager.LogOperation(false, text, decrypted, _key);
                            logger.LogD("Text decrypted", text);
                            break;
                        }
                    }
                }

            }
            catch (InvalidTextException exception)
            {
                Console.WriteLine("!!!!Wrong text!!!!");
                Console.WriteLine("If you want to exit, enter 0.");
                Console.WriteLine(exception);
                errorLogger.LogD(exception.Message, exception);

            }
            catch (InvalidKeyException exception)
            {
                Console.WriteLine("!!!!Wrong key!!!!");
                Console.WriteLine("If you want to exit, enter 0.");
                Console.WriteLine(exception);
                errorLogger.LogD(exception.Message, exception);

            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred:");
                Console.WriteLine(ex);
                errorLogger.LogD(ex.Message, ex);

            }
        }
        var allSessions = _sessionManager.GetAllSessions();
        string directoryPath = @"C:\Users\Ричи\RiderProjects\CipherSolution\CipherLib\Prototype\data";

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, "allSessions.json");

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(allSessions, options);

        File.WriteAllText(filePath, json);

        Console.WriteLine($"Данные успешно сохранены в файл: {filePath}");
    }
}
