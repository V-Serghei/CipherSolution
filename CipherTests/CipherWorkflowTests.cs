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

        string encrypted = workflow.Encrypt("HELLO WORLD");

        Assert.That(encrypted, Is.Not.EqualTo("HELLO WORLD"));
        Assert.That(workflow.Decrypt(encrypted), Is.EqualTo("HELLO WORLD"));
    }

    [Test]
    public void Factory_EncryptDecrypt_Russian()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.Factory,
            CipherType = CipherType.Vigenere,
            Key = "КЛЮЧ"
        });

        string encrypted = workflow.Encrypt("ПРИВЕТ МИР");

        Assert.That(encrypted, Is.Not.EqualTo("ПРИВЕТ МИР"));
        Assert.That(workflow.Decrypt(encrypted), Is.EqualTo("ПРИВЕТ МИР"));
    }

    [Test]
    public void Factory_RussianTextWithEnglishKey_ThrowsClearError()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.Factory,
            CipherType = CipherType.Vigenere,
            Key = "KEY"
        });

        var ex = Assert.Throws<ArgumentException>(() => workflow.Encrypt("ПРИВЕТ"));

        Assert.That(ex!.Message, Does.Contain("Russian text requires a Russian key"));
    }

    [Test]
    public void Factory_NumbersOnlyText_ThrowsClearError()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.Factory,
            CipherType = CipherType.Vigenere,
            Key = "KEY"
        });

        var ex = Assert.Throws<ArgumentException>(() => workflow.Encrypt("123"));

        Assert.That(ex!.Message, Does.Contain("includes numbers"));
    }

    [Test]
    public void Factory_KeyWithNumbers_ThrowsClearError()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.Factory,
            CipherType = CipherType.Vigenere,
            Key = "123KEY"
        });

        var ex = Assert.Throws<ArgumentException>(() => workflow.Encrypt("HELLO"));

        Assert.That(ex!.Message, Does.Contain("includes numbers"));
    }

    [Test]
    public void BuilderManual_MixedAlphabet_WorksWithRusEngVariant()
    {
        var workflow = new CipherWorkflow(new CipherWorkflowOptions
        {
            Mode = CipherWorkflowMode.BuilderManual,
            CipherType = CipherType.Vigenere,
            Key = "КEY",
            AlphabetVariant = "rus+eng"
        });

        const string original = "ПРИВЕТ HELLO";
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
            Key = "КЛЮЧ"
        });

        var ex = Assert.Throws<ArgumentException>(() => workflow.Encrypt("ПРИВЕТ"));

        Assert.That(ex!.Message, Does.Contain("English only"));
    }
}
