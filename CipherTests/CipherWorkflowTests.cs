using CipherLib.ConstVal;
using CipherLib.Service;

namespace CipherTests;

public class CipherWorkflowTests
{
    [Test]
    public void Factory_EncryptDecrypt_English()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.Factory,
            CipherType = CipherType.Vigenere,
            Key = "KEY"
        });

        const string original = "HELLO WORLD";
        string encrypted = workflow.Encrypt(original);

        Assert.That(encrypted, Is.Not.EqualTo(original));
        Assert.That(workflow.Decrypt(encrypted), Is.EqualTo(original));
    }

    [Test]
    public void Factory_EncryptDecrypt_Russian()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.Factory,
            CipherType = CipherType.Vigenere,
            Key = "\u041a\u041b\u042e\u0427"
        });

        const string original = "\u041f\u0420\u0418\u0412\u0415\u0422 \u041c\u0418\u0420";
        string encrypted = workflow.Encrypt(original);

        Assert.That(encrypted, Is.Not.EqualTo(original));
        Assert.That(workflow.Decrypt(encrypted), Is.EqualTo(original));
    }

    [Test]
    public void FactoryAuto_EncryptDecrypt_Numbers()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.Factory,
            CipherType = CipherType.Vigenere,
            Key = "123KEY"
        });

        const string original = "123231";
        string encrypted = workflow.Encrypt(original);

        Assert.That(encrypted, Is.Not.EqualTo(original));
        Assert.That(workflow.Decrypt(encrypted), Is.EqualTo(original));
    }

    [Test]
    public void FactoryAuto_EncryptDecrypt_EnglishNumbers()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.Factory,
            CipherType = CipherType.Vigenere,
            Key = "KEY123"
        });

        const string original = "HELLO123";
        string encrypted = workflow.Encrypt(original);

        Assert.That(encrypted, Is.Not.EqualTo(original));
        Assert.That(workflow.Decrypt(encrypted), Is.EqualTo(original));
    }

    [Test]
    public void FactoryAuto_EncryptDecrypt_MixedRussianEnglishNumbers()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.Factory,
            CipherType = CipherType.Vigenere,
            Key = "\u041aEY123"
        });

        const string original = "\u041f\u0420\u0418\u0412\u0415\u0422 HELLO 123";
        string encrypted = workflow.Encrypt(original);

        Assert.That(encrypted, Is.Not.EqualTo(original));
        Assert.That(workflow.Decrypt(encrypted), Is.EqualTo(original));
    }

    [Test]
    public void FactoryAuto_MixedRussianEnglishNumbers_ChangesEverySupportedCharacter()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.Factory,
            CipherType = CipherType.Vigenere,
            Key = "\u041aEY123"
        });

        const string original = "\u041fA1";
        string encrypted = workflow.Encrypt(original);

        Assert.That(encrypted[0], Is.Not.EqualTo(original[0]));
        Assert.That(encrypted[1], Is.Not.EqualTo(original[1]));
        Assert.That(encrypted[2], Is.Not.EqualTo(original[2]));
        Assert.That(workflow.Decrypt(encrypted), Is.EqualTo(original));
    }

    [Test]
    public void ManualEnglishAlphabet_Numbers_ThrowsClearError()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.BuilderManual,
            CipherType = CipherType.Vigenere,
            Key = "KEY",
            AlphabetVariant = "eng"
        });

        var ex = Assert.Throws<ArgumentException>(() => workflow.Encrypt("123"));

        Assert.That(ex!.Message, Does.Contain("does not allow numbers"));
    }

    [Test]
    public void BuilderManual_MixedAlphabet_WorksWithRusEngVariant()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.BuilderManual,
            CipherType = CipherType.Vigenere,
            Key = "\u041aEY",
            AlphabetVariant = "rus+eng"
        });

        const string original = "\u041f\u0420\u0418\u0412\u0415\u0422 HELLO";
        string encrypted = workflow.Encrypt(original);

        Assert.That(encrypted, Is.Not.EqualTo(original));
        Assert.That(workflow.Decrypt(encrypted), Is.EqualTo(original));
    }

    [Test]
    public void BuilderManual_Numbers_WorkWithNumberVariant()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.BuilderManual,
            CipherType = CipherType.Vigenere,
            Key = "123KEY",
            AlphabetVariant = "eng+num"
        });

        const string original = "123";
        string encrypted = workflow.Encrypt(original);

        Assert.That(encrypted, Is.Not.EqualTo(original));
        Assert.That(workflow.Decrypt(encrypted), Is.EqualTo(original));
    }

    [Test]
    public void BuilderDefault_RussianText_ThrowsClearError()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.BuilderDefault,
            CipherType = CipherType.Vigenere,
            Key = "\u041a\u041b\u042e\u0427"
        });

        var ex = Assert.Throws<ArgumentException>(() => workflow.Encrypt("\u041f\u0420\u0418\u0412\u0415\u0422"));

        Assert.That(ex!.Message, Does.Contain("English only"));
    }
}
