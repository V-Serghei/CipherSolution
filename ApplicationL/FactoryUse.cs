using ApplicationL.CustomExceptions;
using CipherLib;
using CipherLib.Factory;
using CipherLib.Service;
using Logging;

namespace ApplicationL;

public class FactoryUse()
{
    public static ProcessLogger logger = ProcessLogger.Instance;
    public static ErrorLogger errorLogger = ErrorLogger.Instance;
    public static string key = "";
    private ICipher _cipher  = null!;


    public void Run()
    {

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
                logger.Log("Exit");
                return;
            }

            try
            {
                if (string.IsNullOrEmpty(choice))
                {
                    throw new InvalidCipherChoiceException("Invalid cipher choice");
                }

                CipherCreator creator = CipherFactory.GetCipherCreator(choice);
                logger.Log("Cipher created", choice);
                Console.WriteLine("Enter the key:");
                key = Console.ReadLine();
                if (string.IsNullOrEmpty(key))
                {
                    throw new InvalidKeyException("Invalid key");
                }

                logger.Log("Cipher created", key);
                _cipher = creator.CreateCipher(key);
                break;

            }
            catch (InvalidCipherChoiceException exception)
            {
                Console.WriteLine("!!!!Wrong choice!!!!");
                Console.WriteLine("If you want to exit, enter 0.");
                errorLogger.LogError(exception.Message, exception);
            }
            catch (InvalidKeyException exception)
            {
                Console.WriteLine("!!!!Wrong key!!!!");
                Console.WriteLine("If you want to exit, enter 0.");
                Console.WriteLine(exception);
                errorLogger.LogError(exception.Message, exception);

            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred:");
                Console.WriteLine(ex);
                errorLogger.LogError(ex.Message, ex);
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
                logger.Log("Exit");
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
                        key = Console.ReadLine();
                        if (string.IsNullOrEmpty(key))
                            throw new InvalidKeyException("Invalid key");
                        _cipher.SetKey(key);
                        logger.Log("Key changed", key);
                        break;
                    case "00":
                        logger.Log("Cipher changed");
                        goto choiceCipher;
                    default:
                        Console.WriteLine("!!!!!!!Invalid command!!!!!!!!!");
                        break;
                }


                if (input != "3" && !string.IsNullOrEmpty(input) && input is "1" or "2")
                {
                    text = Console.ReadLine();
                    if (string.IsNullOrEmpty(text))
                    {
                        throw new InvalidTextException("Invalid text");
                    }

                    switch (input)
                    {
                        case "1":
                        {
                            var encrypted = service.EncryptText(text);
                            Console.WriteLine($"Encrypted text: {encrypted}");
                            logger.Log("Text encrypted", text);
                            break;
                        }
                        case "2":
                        {
                            var decrypted = service.DecryptText(text);
                            Console.WriteLine($"Decrypted text: {decrypted}");
                            logger.Log("Text decrypted", text);
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
                errorLogger.LogError(exception.Message, exception);

            }
            catch (InvalidKeyException exception)
            {
                Console.WriteLine("!!!!Wrong key!!!!");
                Console.WriteLine("If you want to exit, enter 0.");
                Console.WriteLine(exception);
                errorLogger.LogError(exception.Message, exception);

            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred:");
                Console.WriteLine(ex);
                errorLogger.LogError(ex.Message, ex);

            }
        }
    }
}
