namespace CipherLib;

public interface ICipher
{
    string Encrypt(string text);
    string Decrypt(string text);
    void SetKey(string key);
}