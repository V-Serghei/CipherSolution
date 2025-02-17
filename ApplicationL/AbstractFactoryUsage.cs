using System;
using CipherLib;
using CipherLib.AbstractFactory;
using CipherLib.AbstractFactory.AbstractFactoryConcrete;
using CipherLib.Entities;
using CipherLib.Prototype;
using CipherLib.Service;
using Logging;

namespace ApplicationL
{
    public class AbstractFactoryUsage
    {
        private readonly ILogger _logger = ProcessLoggerLazy.Instance; 
        private ICipher _cipher = null!;
        private EncryptionSessionManager _sessionManager = null!;

        public void Run()
        {
            Console.WriteLine("Select encryption algorithm (Abstract Factory):");
            Console.WriteLine("1. Vigenère cipher");
            Console.WriteLine("2. Beaufort cipher");
            Console.WriteLine("3. Autokey cipher");
            Console.WriteLine("4. Running key cipher");
            string? mode = Console.ReadLine();

            CipherOptions options = new CipherOptions(
                useExplicitAlphabet: false,
                alphabetVariant: "eng",
                allowSymbols: false,
                allowNumbers: false,
                errorLogging: true,
                processLogging: true
            );

            ICipherComponentsFactory factory = mode switch
            {
                "1" => new VigenereCipherComponentsFactory(),
                "2" => new BeaufortCipherComponentsFactory(),
                "3" => new AutoKeyCipherComponentsFactory(),
                "4" => new RunningKeyCipherComponentsFactory(),
                _   => throw new Exception("Invalid mode selected.")
            };

            Console.Write("Enter the key: ");
            string key = Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(key))
            {
                Console.WriteLine("Invalid key");
                return;
            }

            ICipherConfiguration config = factory.CreateConfiguration(key, options);
            _cipher = factory.CreateCipher(config);
            _sessionManager = new EncryptionSessionManager(key);

            RunCipherOperations();
        }

        private void RunCipherOperations()
        {
            CipherService service = new CipherService(_cipher);
            while (true)
            {
                Console.WriteLine("\nSelect an action:");
                Console.WriteLine("1. Encrypt text");
                Console.WriteLine("2. Decrypt text");
                Console.WriteLine("3. Change key");
                Console.WriteLine("00. Choose cipher");
                Console.WriteLine("0. Exit");

                string? input = Console.ReadLine();
                if (input == "0")
                {
                    _logger.LogD("Exit", exception: new Exception("Exit"));
                    break;
                }

                try
                {
                    switch (input)
                    {
                        case "1":
                            Console.Write("Enter text to encrypt: ");
                            string plainText = Console.ReadLine() ?? "";
                            if (string.IsNullOrEmpty(plainText))
                                throw new Exception("Invalid text");
                            string encrypted = service.EncryptText(plainText);
                            Console.WriteLine($"Encrypted: {encrypted}");
                            _sessionManager.LogOperation(true, plainText, encrypted, _cipher.ToString());
                            _logger.LogD("Text encrypted: " + plainText, exception: new Exception("Text encrypted: " + plainText));
                            break;
                        case "2":
                            Console.Write("Enter text to decrypt: ");
                            string cipherText = Console.ReadLine() ?? "";
                            if (string.IsNullOrEmpty(cipherText))
                                throw new Exception("Invalid text");
                            string decrypted = service.DecryptText(cipherText);
                            Console.WriteLine($"Decrypted: {decrypted}");
                            _sessionManager.LogOperation(false, cipherText, decrypted, _cipher.ToString());
                            _logger.LogD("Text decrypted: " + cipherText, exception: new Exception("Text decrypted: " + cipherText));
                            break;
                        case "3":
                            Console.Write("Enter new key: ");
                            string newKey = Console.ReadLine() ?? "";
                            if (string.IsNullOrEmpty(newKey))
                                throw new Exception("Invalid key");
                            _cipher.SetKey(newKey);
                            _sessionManager = new EncryptionSessionManager(newKey);
                            _logger.LogD("Key changed: " + newKey, exception: new Exception("Key changed: " + newKey));
                            break;
                        case "00":
                            _logger.LogD("Cipher changed", exception: new Exception("Cipher changed"));;
                            Run();
                            return;
                        default:
                            Console.WriteLine("Invalid command");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
    }
}
