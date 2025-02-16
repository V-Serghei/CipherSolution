using CipherLib.CipherCore;

namespace CipherLib.Factory.Creators;

public class RunningKeyCipherCreator: CipherCreator
{
    public override ICipher CreateCipher(string key) => new RunningKeyCipher(key);
}