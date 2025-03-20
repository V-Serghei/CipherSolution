using ApplicationL.CustomExceptions;
using CipherLib;
using CipherLib.Builder;
using CipherLib.ConstVal;
using Logging;

namespace ApplicationL;

public class BuilderModeUse
{
     private ICipher _cipher;
     public static ProcessLogger logger = ProcessLogger.Instance;
     public static ILogger errorLogger = ErrorLogger.Instance;
     public static string key = "";
    public void Run()
    {
        choiceCipher:

        Console.WriteLine("select:");
        Console.WriteLine("1. Use standard set (director)");
        Console.WriteLine("2. Manual setting");
        string? builderMode = Console.ReadLine();

        ICipherBuilder builder = new CipherBuilder();
        CipherDirector director = new CipherDirector();

        if (builderMode == "1")
        {
            Console.WriteLine("Select the algorithm to build:");
            Console.WriteLine("1. Vigenère");
            Console.WriteLine("2. Beaufort");
            Console.WriteLine("3. Autokey");
            Console.WriteLine("4. Running key");
            
            
            string? algChoice = Console.ReadLine();
            Console.WriteLine("enter the key:");
            key = Console.ReadLine()!;
            switch (algChoice)
            {
                case "1":
                    director.BuildDefaultVigenere(builder,key);
                    break;
                case "2":
                    director.BuildDefaultBeaufort(builder,key);
                    break;
                case "3":
                    director.BuildDefaultAutoKey(builder,key);
                    break;
                case "4":
                    director.BuildDefaultRunningKey(builder,key);
                    break;
                default:
                    Console.WriteLine("Wrong choice, default Vigenère is used");
                    director.BuildDefaultVigenere(builder);
                    break;
            }
        }
        else
        {
            Console.WriteLine("Select the algorithm type:");
            Console.WriteLine("1. Vigenère");
            Console.WriteLine("2. Beaufort");
            Console.WriteLine("3. Autokey");
            Console.WriteLine("4. Running key");
            string? algChoice = Console.ReadLine();
            switch (algChoice)
            {
                case "1":
                    builder.SetAlgorithmType(CipherType.Vigenere);
                    break;
                case "2":
                    builder.SetAlgorithmType(CipherType.Beaufort);
                    break;
                case "3":
                    builder.SetAlgorithmType(CipherType.AutoKey);
                    break;
                case "4":
                    builder.SetAlgorithmType(CipherType.RunningKey);
                    break;
                default:
                    Console.WriteLine("Wrong choice, default Vigenère is used");
                    builder.SetAlgorithmType(CipherType.Vigenere);
                    break;
            }

            Console.Write("Enter key: ");
            builder.SetKey(Console.ReadLine()!);
            Console.Write("Enter salt (or leave empty): ");
            builder.SetSalt(Console.ReadLine()!);
            Console.Write("Allow encryption of symbols? (true/false): ");
            builder.AllowSymbols(Console.ReadLine()?.ToLower() == "true");
            Console.Write("Allow encryption of numbers? (true/false): ");
            builder.AllowNumbers(Console.ReadLine()?.ToLower() == "true");
            Console.Write("Select language (e.g., eng, rus, rus+eng; if allowed in previous steps you can choose: \n" +
                          "rus+eng+num+sym, rus+num, rus+sym, eng+num, eng+sym, rus+num+sym, eng+num+sym): ");
            builder.SetLanguage(Console.ReadLine()!);
            Console.Write("Enable error logging? (true/false): ");
            builder.EnableErrorLogging(Console.ReadLine()?.ToLower() == "true");
            Console.Write("Enable process logging? (true/false): ");
            builder.EnableProcessLogging(Console.ReadLine()?.ToLower() == "true");

        }

        try
        {
            _cipher = builder.Build();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during compilation of algorithm: {ex.Message}");
            
        }
        
        
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
                        key = Console.ReadLine()!;
                        if (string.IsNullOrEmpty(key))
                            throw new InvalidKeyException("Invalid key");
                        _cipher.SetKey(key);
                        ProcessLogger.Instance.LogD("Key changed", key);
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
                    if (string.IsNullOrEmpty(text))
                    {
                        throw new InvalidTextException("Invalid text");
                    }

                    switch (input)
                    {
                        case "1":
                        {
                            var encrypted = _cipher.Encrypt(text);
                            Console.WriteLine($"Encrypted text: {encrypted}");
                            logger.LogD("Text encrypted", text);
                            break;
                        }
                        case "2":
                        {
                            var decrypted = _cipher.Decrypt(text);
                            Console.WriteLine($"Decrypted text: {decrypted}");
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

    }
}

