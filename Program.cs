using System;
using System.Linq;

namespace Vigenere_cipher
{
public class VigenereCipher
{
    private static readonly char[] ABCrus = new char[]
    {
        'А','Б','В','Г','Д','Е','Ж','З','И','Й','К','Л','М','Н','О','П','Р','С','Т','У','Ф','Х','Ц','Ч','Ш','Щ','Ъ','Ы','Ь','Э','Ю','Я',
        'а','б','в','г','д','е','ж','з','и','й','к','л','м','н','о','п','р','с','т','у','ф','х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я',
        ' '
    };
    
    private static readonly char[] ABCeng = new char[]
    {
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        ' '
    };
    private string _key;
    private char[] _alphabet;

    
    private char[] DetermineAlphabet(string text)
    {
        bool containsRussian = text.Any(c => (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'));
        return containsRussian ? ABCrus : ABCeng;
    }

    public VigenereCipher(string key)
    {
        _key = key; 
        _alphabet = DetermineAlphabet(key);
    }

    public string Encrypt(string text)
    {
        
        _alphabet = DetermineAlphabet(text);
        return Process(text, _key, true);
    }

    public string Decrypt(string text)
    {
        _alphabet = DetermineAlphabet(text);
        return Process(text, _key, false);
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

            
            if (keyIndex == -1)
                keyIndex = 0;

            int newIndex = encrypt
                ? (textIndex + keyIndex) % alphabetLength
                : (textIndex - keyIndex + alphabetLength) % alphabetLength;

            result += _alphabet[newIndex];
            j++; 
        }

        return result;
    }

    public void SetKey(string key)
    {
        _key = key;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var vigenereEng = new VigenereCipher("Key");
        var textEng = "fFDFDFWEFWEFWER";
        var encryptedTextEng = vigenereEng.Encrypt(textEng);
        Console.WriteLine($"Зашифрованный текст: {encryptedTextEng}");
        var decryptedTextEng = vigenereEng.Decrypt(encryptedTextEng);
        Console.WriteLine($"Расшифрованный текст: {decryptedTextEng}");

        var vigenereRus = new VigenereCipher("Ключ");
        var textRus = "Прощай Друг";
        var encryptedTextRus = vigenereRus.Encrypt(textRus);
        Console.WriteLine($"Зашифрованный текст: {encryptedTextRus}");
        var decryptedTextRus = vigenereRus.Decrypt(encryptedTextRus);
        Console.WriteLine($"Расшифрованный текст: {decryptedTextRus}");
    }
}
}
