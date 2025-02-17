namespace CipherLib.Prototype;

public class EncryptionSession: IPrototype<EncryptionSession>
{
   
    public string key { get; set; }
    public List<EncryptionOperation> operations { get; private set; }
    
    
    public EncryptionSession(string key)
    {
        this.key = key;
        this.operations = new List<EncryptionOperation>();
    }
    
    
    public void AddOperation(EncryptionOperation op)
    {
        operations.Add(op);
    }
        
    public EncryptionSession Clone()
    {
        var clone = new EncryptionSession(this.key);
        foreach (var op in this.operations)
        {
            clone.operations.Add(op.Clone());
        }
        return clone;
    }
}