using CipherLib.CipherCore;

namespace CipherTests;

public class CipherTests
{
    // ── Vigenère ─────────────────────────────────────────────────────────────

    [Test]
    public void Vigenere_Encrypt_KnownVector_English()
    {
        // Verified by hand: EngAlphabet = A-Za-z (52 chars)
        // A(0)+L(11)=11='L', T(19)+E(4)=23='X', T(19)+M(12)=31='f',
        // A(0)+O(14)=14='O', C(2)+N(13)=15='P', K(10)+L(11)=21='V'
        var cipher = new VigenereCipher("LEMON");
        Assert.That(cipher.Encrypt("ATTACK"), Is.EqualTo("LXfOPV"));
    }

    [Test]
    public void Vigenere_Decrypt_KnownVector_English()
    {
        var cipher = new VigenereCipher("LEMON");
        Assert.That(cipher.Decrypt("LXfOPV"), Is.EqualTo("ATTACK"));
    }

    [Test]
    public void Vigenere_Roundtrip_English()
    {
        var cipher = new VigenereCipher("KEYWORD");
        const string original = "HelloWorld";
        Assert.That(cipher.Decrypt(cipher.Encrypt(original)), Is.EqualTo(original));
    }

    [Test]
    public void Vigenere_Roundtrip_Russian()
    {
        var cipher = new VigenereCipher("КЛЮЧ");
        const string original = "ПРИВЕТ";
        Assert.That(cipher.Decrypt(cipher.Encrypt(original)), Is.EqualTo(original));
    }

    [Test]
    public void Vigenere_NonAlphabetChars_PassThrough()
    {
        // '!' is not in EngAlphabet (A-Za-z + space), so it must pass through unchanged
        var cipher = new VigenereCipher("KEY");
        string encrypted = cipher.Encrypt("HELLO!WORLD");
        Assert.That(encrypted, Does.Contain('!'));
    }

    [Test]
    public void Vigenere_EmptyString_ReturnsEmpty()
    {
        var cipher = new VigenereCipher("KEY");
        Assert.That(cipher.Encrypt(""), Is.EqualTo(""));
        Assert.That(cipher.Decrypt(""), Is.EqualTo(""));
    }

    // ── Beaufort ──────────────────────────────────────────────────────────────

    [Test]
    public void Beaufort_IsSymmetric()
    {
        // Beaufort's defining property: Encrypt(text) == Decrypt(text)
        var cipher = new BeaufortCipher("SECRET");
        const string text = "PLAINTEXT";
        Assert.That(cipher.Encrypt(text), Is.EqualTo(cipher.Decrypt(text)));
    }

    [Test]
    public void Beaufort_Roundtrip_English()
    {
        var cipher = new BeaufortCipher("KEYWORD");
        const string original = "HELLOWORLD";
        Assert.That(cipher.Decrypt(cipher.Encrypt(original)), Is.EqualTo(original));
    }

    [Test]
    public void Beaufort_Roundtrip_Russian()
    {
        var cipher = new BeaufortCipher("КЛЮЧ");
        const string original = "ПРИВЕТ";
        Assert.That(cipher.Decrypt(cipher.Encrypt(original)), Is.EqualTo(original));
    }

    // ── AutoKey ───────────────────────────────────────────────────────────────

    [Test]
    public void AutoKey_Roundtrip_English()
    {
        var cipher = new AutoKeyCipher("SECRET");
        const string original = "HELLOWORLD";
        Assert.That(cipher.Decrypt(cipher.Encrypt(original)), Is.EqualTo(original));
    }

    [Test]
    public void AutoKey_Roundtrip_TextLongerThanKey()
    {
        var cipher = new AutoKeyCipher("AB");
        const string original = "LONGERTHANKEY";
        Assert.That(cipher.Decrypt(cipher.Encrypt(original)), Is.EqualTo(original));
    }

    [Test]
    public void AutoKey_Roundtrip_Russian()
    {
        var cipher = new AutoKeyCipher("КЛЮЧ");
        const string original = "ПРИВЕТ";
        Assert.That(cipher.Decrypt(cipher.Encrypt(original)), Is.EqualTo(original));
    }

    // ── RunningKey ────────────────────────────────────────────────────────────

    [Test]
    public void RunningKey_Roundtrip_English()
    {
        var cipher = new RunningKeyCipher("THEQUICKBROWNFOXJUMPSOVERTHELAZYDOG");
        const string original = "HELLOWORLD";
        Assert.That(cipher.Decrypt(cipher.Encrypt(original)), Is.EqualTo(original));
    }

    [Test]
    public void RunningKey_Roundtrip_Russian()
    {
        var cipher = new RunningKeyCipher("ТЕСТОВЫЙКЛЮЧДЛЯРУССКОГОТЕКСТА");
        const string original = "ПРИВЕТ";
        Assert.That(cipher.Decrypt(cipher.Encrypt(original)), Is.EqualTo(original));
    }

    [Test]
    public void RunningKey_Encrypt_ThrowsWhenKeyTooShort()
    {
        var cipher = new RunningKeyCipher("SHORT");
        Assert.Throws<ArgumentException>(() => cipher.Encrypt("MUCHLONGERTEXT"));
    }

    [Test]
    public void RunningKey_Decrypt_ThrowsWhenKeyTooShort()
    {
        var cipher = new RunningKeyCipher("SHORT");
        Assert.Throws<ArgumentException>(() => cipher.Decrypt("MUCHLONGERTEXT"));
    }
}
