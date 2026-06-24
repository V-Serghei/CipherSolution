using System.Text.Json;
using CipherLib.AbstractFactory;
using CipherLib.AbstractFactory.AbstractFactoryConcrete;
using CipherLib.Builder;
using CipherLib.ConstVal;
using CipherLib.Entities;
using CipherLib.Factory;
using CipherLib.Prototype;

namespace CipherLib.Service;

public enum CipherWorkflowMode
{
    Factory,
    BuilderDefault,
    BuilderManual,
    AbstractFactory
}

public sealed class CipherWorkflowOptions
{
    public CipherWorkflowMode Mode { get; init; } = CipherWorkflowMode.Factory;
    public CipherType CipherType { get; init; } = CipherType.Vigenere;
    public string Key { get; init; } = "";
    public string Salt { get; init; } = "";
    public string AlphabetVariant { get; init; } = "eng";
    public bool ErrorLogging { get; init; }
    public bool ProcessLogging { get; init; }
}

public sealed class CipherWorkflow
{
    private readonly CipherWorkflowOptions _options;
    private readonly ICipher _cipher;
    private readonly CipherService _service;
    private readonly EncryptionSessionManager _sessionManager;

    public CipherWorkflow(CipherWorkflowOptions options)
    {
        _options = options;

        if (string.IsNullOrWhiteSpace(options.Key))
        {
            throw new ArgumentException("Key cannot be empty.");
        }

        _cipher = CreateCipher(options);
        _service = new CipherService(_cipher);
        _sessionManager = new EncryptionSessionManager(options.Key);
    }

    public string Encrypt(string text)
    {
        ValidateInput(text, isEncryption: true);
        string output = _service.EncryptText(text);
        _sessionManager.LogOperation(true, text, output, _options.Key);
        SaveSessions();
        return output;
    }

    public string Decrypt(string text)
    {
        ValidateInput(text, isEncryption: false);
        string output = _service.DecryptText(text);
        _sessionManager.LogOperation(false, text, output, _options.Key);
        SaveSessions();
        return output;
    }

    private static ICipher CreateCipher(CipherWorkflowOptions options)
    {
        return options.Mode switch
        {
            CipherWorkflowMode.Factory => CreateFactoryCipher(options),
            CipherWorkflowMode.BuilderDefault => CreateDefaultBuilderCipher(options),
            CipherWorkflowMode.BuilderManual => CreateManualBuilderCipher(options),
            CipherWorkflowMode.AbstractFactory => CreateAbstractFactoryCipher(options),
            _ => throw new InvalidOperationException("Unknown cipher workflow mode.")
        };
    }

    private static ICipher CreateFactoryCipher(CipherWorkflowOptions options)
    {
        return CipherFactory.GetCipherCreator(GetCipherFactoryChoice(options.CipherType)).CreateCipher(options.Key);
    }

    private static ICipher CreateDefaultBuilderCipher(CipherWorkflowOptions options)
    {
        ICipherBuilder builder = new CipherBuilder();
        CipherDirector director = new CipherDirector();

        switch (options.CipherType)
        {
            case CipherType.Vigenere:
                director.BuildDefaultVigenere(builder, options.Key);
                break;
            case CipherType.Beaufort:
                director.BuildDefaultBeaufort(builder, options.Key);
                break;
            case CipherType.AutoKey:
                director.BuildDefaultAutoKey(builder, options.Key);
                break;
            case CipherType.RunningKey:
                director.BuildDefaultRunningKey(builder, options.Key);
                break;
            default:
                throw new ArgumentException("Unknown cipher type.");
        }

        return builder.Build();
    }

    private static ICipher CreateManualBuilderCipher(CipherWorkflowOptions options)
    {
        ICipherBuilder builder = new CipherBuilder()
            .SetAlgorithmType(options.CipherType)
            .SetKey(options.Key)
            .SetSalt(options.Salt)
            .SetLanguage(options.AlphabetVariant)
            .AllowSymbols(VariantUsesSymbols(options.AlphabetVariant))
            .AllowNumbers(VariantUsesNumbers(options.AlphabetVariant))
            .EnableErrorLogging(options.ErrorLogging)
            .EnableProcessLogging(options.ProcessLogging);

        return builder.Build();
    }

    private static ICipher CreateAbstractFactoryCipher(CipherWorkflowOptions options)
    {
        ICipherComponentsFactory factory = options.CipherType switch
        {
            CipherType.Vigenere => new VigenereCipherComponentsFactory(),
            CipherType.Beaufort => new BeaufortCipherComponentsFactory(),
            CipherType.AutoKey => new AutoKeyCipherComponentsFactory(),
            CipherType.RunningKey => new RunningKeyCipherComponentsFactory(),
            _ => throw new ArgumentException("Unknown cipher type.")
        };

        CipherOptions cipherOptions = new(
            useExplicitAlphabet: false,
            alphabetVariant: "eng",
            allowSymbols: false,
            allowNumbers: false,
            errorLogging: true,
            processLogging: true);

        return factory.CreateCipher(factory.CreateConfiguration(options.Key, cipherOptions));
    }

    private void ValidateInput(string text, bool isEncryption)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be empty.");
        }

        string effectiveKey = _options.Mode == CipherWorkflowMode.BuilderManual
            ? _options.Key + _options.Salt
            : _options.Key;

        if (_options.CipherType == CipherType.RunningKey && effectiveKey.Length < text.Length)
        {
            string target = isEncryption ? "text to encrypt" : "ciphertext";
            throw new ArgumentException($"The running key must be no shorter than the {target}.");
        }

        if (_options.Mode == CipherWorkflowMode.BuilderDefault)
        {
            ValidateExplicitAlphabet(text, effectiveKey, "eng");
            return;
        }

        if (_options.Mode == CipherWorkflowMode.BuilderManual)
        {
            ValidateExplicitAlphabet(text, effectiveKey, _options.AlphabetVariant);
            return;
        }

        ValidateAutoDetectedAlphabet(text, effectiveKey);
    }

    private static void ValidateAutoDetectedAlphabet(string text, string key)
    {
        if (HasDigits(text + key))
        {
            throw new ArgumentException("Factory and Abstract Factory modes encrypt letters only. Use Builder - Manual with an alphabet variant that includes numbers.");
        }

        if (HasSymbols(text + key))
        {
            throw new ArgumentException("Factory and Abstract Factory modes encrypt letters and spaces only. Use Builder - Manual with an alphabet variant that includes symbols.");
        }

        bool textHasRussian = HasRussianLetters(text);
        bool textHasEnglish = HasEnglishLetters(text);

        if (!textHasRussian && !textHasEnglish)
        {
            throw new ArgumentException("Text must contain letters to be encrypted in this mode.");
        }

        if (textHasRussian && textHasEnglish)
        {
            throw new ArgumentException("Factory and Abstract Factory modes use one detected alphabet. Use Builder - Manual with rus+eng for mixed Russian and English text.");
        }

        if (textHasRussian && !HasRussianLetters(key))
        {
            throw new ArgumentException("Russian text requires a Russian key. Example: КЛЮЧ.");
        }

        if (textHasEnglish && !HasEnglishLetters(key))
        {
            throw new ArgumentException("English text requires an English key. Example: KEY.");
        }
    }

    private static void ValidateExplicitAlphabet(string text, string key, string alphabetVariant)
    {
        if (alphabetVariant.Contains("rus", StringComparison.OrdinalIgnoreCase) &&
            !alphabetVariant.Contains("eng", StringComparison.OrdinalIgnoreCase) &&
            HasEnglishLetters(text + key))
        {
            throw new ArgumentException("The selected alphabet variant is Russian only, but the text or key contains English letters.");
        }

        if (alphabetVariant.Contains("eng", StringComparison.OrdinalIgnoreCase) &&
            !alphabetVariant.Contains("rus", StringComparison.OrdinalIgnoreCase) &&
            HasRussianLetters(text + key))
        {
            throw new ArgumentException("The selected alphabet variant is English only, but the text or key contains Russian letters.");
        }

        if (!VariantUsesNumbers(alphabetVariant) && HasDigits(text + key))
        {
            throw new ArgumentException("The selected alphabet variant does not allow numbers.");
        }

        if (!VariantUsesSymbols(alphabetVariant) && HasSymbols(text + key))
        {
            throw new ArgumentException("The selected alphabet variant does not allow symbols.");
        }
    }

    private void SaveSessions()
    {
        string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "sessions");
        Directory.CreateDirectory(directoryPath);

        string filePath = Path.Combine(directoryPath, "allSessions.json");
        string json = JsonSerializer.Serialize(
            _sessionManager.GetAllSessions(),
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(filePath, json);
    }

    private static string GetCipherFactoryChoice(CipherType cipherType)
    {
        return cipherType switch
        {
            CipherType.Vigenere => "vigenere",
            CipherType.Beaufort => "beaufort",
            CipherType.AutoKey => "autokey",
            CipherType.RunningKey => "runningkey",
            _ => throw new ArgumentException("Unknown cipher type.")
        };
    }

    private static bool HasRussianLetters(string value)
    {
        return value.Any(c => (c >= 'А' && c <= 'я') || c == 'Ё' || c == 'ё');
    }

    private static bool HasEnglishLetters(string value)
    {
        return value.Any(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'));
    }

    private static bool HasDigits(string value)
    {
        return value.Any(char.IsDigit);
    }

    private static bool HasSymbols(string value)
    {
        return value.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
    }

    private static bool VariantUsesNumbers(string variant)
    {
        return variant.Contains("num", StringComparison.OrdinalIgnoreCase);
    }

    private static bool VariantUsesSymbols(string variant)
    {
        return variant.Contains("sym", StringComparison.OrdinalIgnoreCase);
    }
}
