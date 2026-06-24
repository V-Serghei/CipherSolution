namespace CipherLib.Practical;

public interface ITextCipherStrategy
{
    string DisplayName { get; }
    string LastDetails { get; }
    string Encrypt(string input);
    string Decrypt(string input);
}
