namespace CipherLib;

/// <summary>
/// Interface for cipher classes
/// Factory pattern is used to create cipher objects
/// ICipher is a simple abstract product 
/// </summary>
public interface ICipher
{
    string Encrypt(string text);
    string Decrypt(string text);
    void SetKey(string key);
}