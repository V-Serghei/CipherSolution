using CipherLib.CipherCore;

namespace CipherLib.Factory.Creators;

public class AutoKeyCipherCreator: CipherCreator
{
    public override ICipher CreateCipher(string key) => new AutoKeyCipher(key);
}