namespace CipherLib.Factory;

public abstract class CipherCreator
{
    public abstract ICipher CreateCipher(string key);
    
}