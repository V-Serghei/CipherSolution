using System;
using CipherLib.ConstVal;

namespace CipherLib.Builder
{
    public class CipherDirector
    {
        public void BuildDefaultVigenere(ICipherBuilder builder, string key = "Default")
        {
            builder.SetAlgorithmType(CipherType.Vigenere)
                .SetKey(key)
                .SetLanguage("eng")
                .AllowSymbols(false)
                .AllowNumbers(false)
                .EnableErrorLogging(true)
                .EnableProcessLogging(true);
        }

        public void BuildDefaultBeaufort(ICipherBuilder builder,string key = "Default")
        {
            builder.SetAlgorithmType(CipherType.Beaufort)
                .SetKey(key)
                .SetLanguage("eng")
                .AllowSymbols(false)
                .AllowNumbers(false)
                .EnableErrorLogging(true)
                .EnableProcessLogging(true);
        }

        public void BuildDefaultRunningKey(ICipherBuilder builder,string key = "Default")
        {
            builder.SetAlgorithmType(CipherType.RunningKey).SetKey("DefaultRunningKey")
                .SetKey(key)
                .SetLanguage("eng")
                .AllowSymbols(false)
                .AllowNumbers(false)
                .EnableErrorLogging(true)
                .EnableProcessLogging(true);
        }

        public void BuildDefaultAutoKey(ICipherBuilder builder,string key = "Default")
        {
            builder.SetAlgorithmType(CipherType.RunningKey).SetKey("DefaultRunningKey")
                .SetKey(key)
                .SetLanguage("eng")
                .AllowSymbols(false)
                .AllowNumbers(false)
                .EnableErrorLogging(true)
                .EnableProcessLogging(true);
        }
        
    }
}