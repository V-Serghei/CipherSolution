namespace CipherLib.Prototype;

public class EncryptionOperation: IPrototype<EncryptionOperation>
{
    
    public DateTime Timestamp { get; set; }
    public bool IsEncrypted { get; set; }
    
    public string InputText { get; set; }
    
    public string OutputText { get; set; }
    
    public EncryptionOperation( bool isEncrypted, string inputText, string outputText)
    {
        Timestamp = DateTime.Now;
        IsEncrypted = isEncrypted;
        InputText = inputText;
        OutputText = outputText;
    }
    
    public EncryptionOperation Clone()
    {
        return new EncryptionOperation(IsEncrypted, InputText, OutputText)
        {
            Timestamp = this.Timestamp
        };
    }
}