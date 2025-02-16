using CipherLib.ConstVal;

namespace CipherLib.Builder;

public interface ICipherBuilder
{
    ICipherBuilder SetAlgorithmType(CipherType type);
    ICipherBuilder SetKey(string key);
    ICipherBuilder SetSalt(string salt); 
    ICipherBuilder SetLanguage(string language); 
    ICipherBuilder AllowSymbols(bool allow);
    ICipherBuilder AllowNumbers(bool allow);
    ICipherBuilder EnableErrorLogging(bool enable);
    ICipherBuilder EnableProcessLogging(bool enable);
    ICipherBuilder SetKeyConstraint(Func<string, bool> constraint, string errorMessage);
    ICipher Build();
}