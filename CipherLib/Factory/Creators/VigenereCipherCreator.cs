using CipherLib.CipherCore;

namespace CipherLib.Factory.Creators;

public class VigenereCipherCreator: CipherCreator
{
    public override ICipher CreateCipher(string key) => new VigenereCipher(key);
}