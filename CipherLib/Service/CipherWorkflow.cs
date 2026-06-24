using System.Text.Json;
using CipherLib.AbstractFactory;
using CipherLib.AbstractFactory.AbstractFactoryConcrete;
using CipherLib.Builder;
using CipherLib.CipherCore;
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
    public string AlphabetVariant { get; init; } = "auto";
    public bool ErrorLogging { get; init; }
    public bool ProcessLogging { get; init; }
}

public sealed class CipherWorkflow
{
    private readonly CipherWorkflowOptions _options;
    private readonly EncryptionSessionManager _sessionManager;

    public CipherWorkflow(CipherWorkflowOptions options)
    {
        _options = options;

        if (string.IsNullOrWhiteSpace(options.Key))
        {
            throw new ArgumentException("Key cannot be empty.");
        }

        _sessionManager = new EncryptionSessionManager(options.Key);
    }

    public string Encrypt(string text)
    {
        string alphabetVariant = ValidateInput(text, isEncryption: true);
        CipherService service = new(CreateCipher(_options, alphabetVariant));
        string output = service.EncryptText(text);
        _sessionManager.LogOperation(true, text, output, _options.Key);
        SaveSessions();
        return output;
    }

    public string Decrypt(string text)
    {
        string alphabetVariant = ValidateInput(text, isEncryption: false);
        CipherService service = new(CreateCipher(_options, alphabetVariant));
        string output = service.DecryptText(text);
        _sessionManager.LogOperation(false, text, output, _options.Key);
        SaveSessions();
        return output;
    }

    private static ICipher CreateCipher(CipherWorkflowOptions options, string alphabetVariant)
    {
        return options.Mode switch
        {
            CipherWorkflowMode.Factory => CreateFactoryCipher(options, alphabetVariant),
            CipherWorkflowMode.BuilderDefault => CreateDefaultBuilderCipher(options),
            CipherWorkflowMode.BuilderManual => CreateManualBuilderCipher(options, alphabetVariant),
            CipherWorkflowMode.AbstractFactory => CreateAbstractFactoryCipher(options, alphabetVariant),
            _ => throw new InvalidOperationException("Unknown cipher workflow mode.")
        };
    }

    private static ICipher CreateFactoryCipher(CipherWorkflowOptions options, string alphabetVariant)
    {
        if (alphabetVariant is "eng" or "rus")
        {
            return CipherFactory.GetCipherCreator(GetCipherFactoryChoice(options.CipherType)).CreateCipher(options.Key);
        }

        return CreateCoreCipher(options.CipherType, options.Key, CreateCipherOptions(options, alphabetVariant));
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

    private static ICipher CreateManualBuilderCipher(CipherWorkflowOptions options, string alphabetVariant)
    {
        ICipherBuilder builder = new CipherBuilder()
            .SetAlgorithmType(options.CipherType)
            .SetKey(options.Key)
            .SetSalt(options.Salt)
            .SetLanguage(alphabetVariant)
            .AllowSymbols(VariantUsesSymbols(alphabetVariant))
            .AllowNumbers(VariantUsesNumbers(alphabetVariant))
            .EnableErrorLogging(options.ErrorLogging)
            .EnableProcessLogging(options.ProcessLogging);

        return builder.Build();
    }

    private static ICipher CreateAbstractFactoryCipher(CipherWorkflowOptions options, string alphabetVariant)
    {
        ICipherComponentsFactory factory = options.CipherType switch
        {
            CipherType.Vigenere => new VigenereCipherComponentsFactory(),
            CipherType.Beaufort => new BeaufortCipherComponentsFactory(),
            CipherType.AutoKey => new AutoKeyCipherComponentsFactory(),
            CipherType.RunningKey => new RunningKeyCipherComponentsFactory(),
            _ => throw new ArgumentException("Unknown cipher type.")
        };

        CipherOptions cipherOptions = CreateCipherOptions(options, alphabetVariant);
        return factory.CreateCipher(factory.CreateConfiguration(options.Key, cipherOptions));
    }

    private string ValidateInput(string text, bool isEncryption)
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
            return "eng";
        }

        string alphabetVariant = ResolveAlphabetVariant(text, effectiveKey, _options.AlphabetVariant);
        ValidateExplicitAlphabet(text, effectiveKey, alphabetVariant);
        return alphabetVariant;
    }

    private static CipherOptions CreateCipherOptions(CipherWorkflowOptions options, string alphabetVariant)
    {
        return new CipherOptions(
            useExplicitAlphabet: true,
            alphabetVariant: alphabetVariant,
            allowSymbols: VariantUsesSymbols(alphabetVariant),
            allowNumbers: VariantUsesNumbers(alphabetVariant),
            errorLogging: options.ErrorLogging,
            processLogging: options.ProcessLogging);
    }

    private static ICipher CreateCoreCipher(CipherType cipherType, string key, CipherOptions options)
    {
        return cipherType switch
        {
            CipherType.Vigenere => new VigenereCipher(key, options),
            CipherType.Beaufort => new BeaufortCipher(key, options),
            CipherType.AutoKey => new AutoKeyCipher(key, options),
            CipherType.RunningKey => new RunningKeyCipher(key, options),
            _ => throw new ArgumentException("Unknown cipher type.")
        };
    }

    private static string ResolveAlphabetVariant(string text, string key, string selectedVariant)
    {
        if (!selectedVariant.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
            return selectedVariant;
        }

        string value = text + key;
        bool hasRussian = HasRussianLetters(value);
        bool hasEnglish = HasEnglishLetters(value);
        bool hasDigits = HasDigits(value);
        bool hasSymbols = HasSymbols(value);

        if (!hasRussian && !hasEnglish && !hasDigits && !hasSymbols)
        {
            throw new ArgumentException("Text or key must contain characters that can be encrypted.");
        }

        string variant = hasRussian && hasEnglish
            ? "rus+eng"
            : hasRussian
                ? "rus"
                : "eng";

        if (hasDigits)
        {
            variant += "+num";
        }

        if (hasSymbols)
        {
            variant += "+sym";
        }

        return variant;
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
        return value.Any(c => (c >= '\u0410' && c <= '\u044f') || c == '\u0401' || c == '\u0451');
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
