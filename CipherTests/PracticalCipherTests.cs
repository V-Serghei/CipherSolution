using CipherLib.ConstVal;
using CipherLib.Practical;
using CipherLib.Service;

namespace CipherTests;

public class PracticalCipherTests
{
    [Test]
    public void AesGcm_EncryptDecrypt_Roundtrip()
    {
        var strategy = new AesGcmTextCipherStrategy("strong password");
        const string original = "Private message 123";

        string encrypted = strategy.Encrypt(original);
        string decrypted = strategy.Decrypt(encrypted);

        Assert.That(encrypted, Is.Not.EqualTo(original));
        Assert.That(encrypted, Does.StartWith("CS-AES-GCM-V1."));
        Assert.That(decrypted, Is.EqualTo(original));
    }

    [Test]
    public void AesGcm_DecryptWithWrongPassword_Throws()
    {
        var encryptor = new AesGcmTextCipherStrategy("right password");
        var decryptor = new AesGcmTextCipherStrategy("wrong password");
        string encrypted = encryptor.Encrypt("Secret");

        Assert.Throws<System.Security.Cryptography.AuthenticationTagMismatchException>(() => decryptor.Decrypt(encrypted));
    }

    [Test]
    public void Facade_ExecutesCommandAndStoresHistoryEntry()
    {
        var facade = new PracticalCipherFacade();
        var strategy = new ClassicalCipherStrategy("Vigenere", new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.Factory,
            CipherType = CipherType.Vigenere,
            Key = "KEY"
        });

        const string input = "HELLO";
        var command = new CipherOperationCommand("Encrypt", () => strategy.Encrypt(input));
        OperationHistoryEntry entry = facade.Execute(strategy, command, "K...Y", "Practical", input);

        Assert.That(entry.Output, Is.Not.EqualTo(input));
        Assert.That(entry.RoundTripOk, Is.True);
        Assert.That(facade.History, Has.Count.EqualTo(1));
        Assert.That(facade.History[0].Operation, Is.EqualTo("Encrypt"));
    }
}
