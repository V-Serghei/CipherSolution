namespace Vigenere_cipher;

public class VigenereСipher
{
    private const int SIZE_ABCrus = 33;
    private const int SIZE_ABCeng = 27;
    private string _key;
    private  char[] _alphabet;

    private static readonly char[] ABCrus = new char[SIZE_ABCrus]
    {
        'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н',
        'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы',
        'ь', 'э', 'ю', 'я', ' '
    };

    private static readonly char[] ABCeng = new char[SIZE_ABCeng]
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
        'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', ' '
    };

    private char[] DetermineAlphabet(string text)
    {
        text = text.ToLower(); 
        return text.Any(c => ABCrus.Contains(c)) ? ABCrus : ABCeng;
    }

    public VigenereСipher(string key)
    {
        _key = key.ToLower(); 
        _alphabet = DetermineAlphabet(key); 
    }

    public string Encrypt(string text)
    {
        _alphabet = DetermineAlphabet(text); 
        return Process(text.ToLower(), _key, true);
    }

    public string Decrypt(string text)
    {
        _alphabet = DetermineAlphabet(text); 
        return Process(text.ToLower(), _key, false);
    }

    private string Process(string text, string key, bool encrypt)
    {
        string result = "";
        int alphabetLength = _alphabet.Length;

        for (int i = 0, j = 0; i < text.Length; i++)
        {
            char textChar = text[i];
            int textIndex = Array.IndexOf(_alphabet, textChar);

            if (textIndex == -1)
            {
                result += textChar; 
                continue;
            }

            char keyChar = key[j % key.Length];
            int keyIndex = Array.IndexOf(_alphabet, keyChar);

            int newIndex = encrypt
                ? (textIndex + keyIndex) % alphabetLength
                : (textIndex - keyIndex + alphabetLength) % alphabetLength;

            result += _alphabet[newIndex];
            j++; 
        }

        return result;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var vigenere = new VigenereСipher("key");
        var text = "text from vigenere cipher";

        var encryptedText = vigenere.Encrypt(text);
        Console.WriteLine($"Зашифрованный текст: {encryptedText}");

        var decryptedText = vigenere.Decrypt(encryptedText);
        Console.WriteLine($"Расшифрованный текст: {decryptedText}");
    }
}
