using CipherLib.CipherCore;
using CipherLib.ConstVal;
using CipherLib.Entities;

namespace CipherLib.Builder;

public class CipherBuilder: ICipherBuilder
{
    
    private CipherType _type = CipherType.Vigenere;
    private string _key = "";
    private string _salt = "";
    private string _language = "eng";
    private bool _allowSymbols = true;
    private bool _allowNumbers = true;
    private bool _enableErrorLogging = false;
    private bool _enableProcessLogging = false;
    private Func<string, bool>? _keyConstraint;
    private string _keyConstraintError = "Invalid key";
    
    public ICipherBuilder SetAlgorithmType(CipherType type)
    {
        _type = type;
        return this;
    }

    public ICipherBuilder SetKey(string key)
    {
        _key = key;
        return this;
    }

    public ICipherBuilder SetSalt(string salt)
    {
        _salt = salt;
        return this;
    }

    public ICipherBuilder SetLanguage(string language)
    {
        _language = language;
        return this;
    }

    public ICipherBuilder AllowSymbols(bool allow)
    {
        _allowSymbols = allow;
        return this;
    }

    public ICipherBuilder AllowNumbers(bool allow)
    {
        _allowNumbers = allow;
        return this;
    }

    public ICipherBuilder EnableErrorLogging(bool enable)
    {
        _enableErrorLogging = enable;
        return this;
    }

    public ICipherBuilder EnableProcessLogging(bool enable)
    {
        _enableProcessLogging = enable;
        return this;
    }

    public ICipherBuilder SetKeyConstraint(Func<string, bool> constraint, string errorMessage)
    {
        _keyConstraint = constraint;
        _keyConstraintError = errorMessage;
        return this;
    }

    public ICipher Build()
    {
        
        if (_keyConstraint != null && !_keyConstraint(_key))
        {
            throw new Exception(_keyConstraintError);
        }

        string effectiveKey = _key;
        if (!string.IsNullOrEmpty(_salt))
        {
            effectiveKey += _salt;
        }

        CipherOptions options = new CipherOptions(true, _language, _allowSymbols, _allowNumbers, _enableErrorLogging,
            _enableProcessLogging);

        ICipher underlyingCipher = _type switch
        {
            CipherType.Vigenere => new VigenereCipher(effectiveKey,options),
            CipherType.Beaufort => new BeaufortCipher(effectiveKey,options),
            CipherType.AutoKey => new AutoKeyCipher(effectiveKey,options),
            CipherType.RunningKey => new RunningKeyCipher(effectiveKey,options),
            _ => throw new Exception("Unsupported cipher type")
        };
        
        return new CustomCipher(underlyingCipher, _language, _enableErrorLogging, _enableProcessLogging);
    }
    
    
}