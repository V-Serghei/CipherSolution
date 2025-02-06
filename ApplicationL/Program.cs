using CipherLib;
using CipherLib.Factory;
using CipherLib.Service;

namespace ApplicationL
{
    class Program
    {
        static void Main(string[] args)
        {
           
            
            string? choice = null;
            ICipher cipher = null;
            string key = "";
            do
            {
                Console.WriteLine("Select encryption algorithm:");
                Console.WriteLine("1. Vigenère cipher");
                Console.WriteLine("2. Beaufort cipher");
                choice = Console.ReadLine();
                if (choice == "1")
                {
                    Console.Write("Enter the key: ");
                    key = Console.ReadLine();
                    cipher = CipherFactory.CreateCipher("vigenere", key);
                }
                else if (choice == "2")
                {
                    Console.Write("Enter the key: ");
                    key = Console.ReadLine();
                    cipher = CipherFactory.CreateCipher("beaufort", key);
                }
                else if (choice == "0")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("!!!!Wrong choice!!!!");
                    choice = null;
                    Console.WriteLine("If you want to exit, enter 0.");
                }
            }while (choice == null);

            var service = new CipherService(cipher);
            
            while (true)
            {
                Console.WriteLine("\nSelect an action:");
                Console.WriteLine("1. Encrypt text");
                Console.WriteLine("2. Decipher the text");
                Console.WriteLine("3. Change key");
                Console.WriteLine("0. Exit");

                var input = Console.ReadLine();
                string? text = null;
                if (input == "0")
                    break;
               
                
                if (input == "1")
                {
                    Console.Write("Enter the text: ");
                    text = Console.ReadLine();
                    var encrypted = service.EncryptText(text);
                    Console.WriteLine($"Encrypted text: {encrypted}");
                }
                else if (input == "2")
                {
                    Console.Write("Enter encrypted text: ");
                    text = Console.ReadLine();
                    var decrypted = service.DecryptText(text);
                    Console.WriteLine($"Decrypted text: {decrypted}");
                }
                else if (input == "3")
                {
                    Console.Write("Enter new key: ");
                    key = Console.ReadLine();
                    cipher.SetKey(key);
                }
                else
                {
                    Console.WriteLine("!!!!!!!Invalid command!!!!!!!!!");
                }
            }
        }
    }
}
