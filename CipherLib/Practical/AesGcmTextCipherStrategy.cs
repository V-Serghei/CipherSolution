using System.Security.Cryptography;
using System.Text;

namespace CipherLib.Practical;

public sealed class AesGcmTextCipherStrategy : ITextCipherStrategy
{
    private const int SaltSize = 16;
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 210_000;
    private const string Prefix = "CS-AES-GCM-V1";
    private readonly string _password;

    public AesGcmTextCipherStrategy(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty.");
        }

        _password = password;
    }

    public string DisplayName => "AES-GCM";
    public string LastDetails => "AES-GCM with PBKDF2-SHA256";

    public string Encrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Text cannot be empty.");
        }

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
        byte[] key = DeriveKey(_password, salt);
        byte[] plain = Encoding.UTF8.GetBytes(input);
        byte[] cipher = new byte[plain.Length];
        byte[] tag = new byte[TagSize];

        using var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, plain, cipher, tag);

        return string.Join(".",
            Prefix,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(nonce),
            Convert.ToBase64String(cipher),
            Convert.ToBase64String(tag));
    }

    public string Decrypt(string input)
    {
        string[] parts = input.Split('.');
        if (parts.Length != 5 || parts[0] != Prefix)
        {
            throw new ArgumentException("Input is not a CipherSolution AES-GCM payload.");
        }

        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] nonce = Convert.FromBase64String(parts[2]);
        byte[] cipher = Convert.FromBase64String(parts[3]);
        byte[] tag = Convert.FromBase64String(parts[4]);
        byte[] key = DeriveKey(_password, salt);
        byte[] plain = new byte[cipher.Length];

        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, cipher, tag, plain);

        return Encoding.UTF8.GetString(plain);
    }

    private static byte[] DeriveKey(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: KeySize);
    }
}
