using CipherLib.CipherCore;

namespace CipherLib.Factory.Creators;

public class BeaufortCipherCreator: CipherCreator
{
    public override ICipher CreateCipher(string key) => new BeaufortCipher(key);
}