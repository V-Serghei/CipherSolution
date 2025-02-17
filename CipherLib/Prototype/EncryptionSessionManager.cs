namespace CipherLib.Prototype;

public class EncryptionSessionManager
{
    private List<EncryptionSession> _archivedSessions;
    private EncryptionSession _currentSession;
        
    public EncryptionSessionManager(string initialKey)
    {
        _archivedSessions = new List<EncryptionSession>();
        _currentSession = new EncryptionSession(initialKey);
    }
        
    
    public void LogOperation(bool isEncryption, string input, string output, string currentKey)
    {
        if (_currentSession.key != currentKey)
        {
            _archivedSessions.Add(_currentSession.Clone());
            _currentSession = new EncryptionSession(currentKey);
        }
            
        _currentSession.AddOperation(new EncryptionOperation(isEncryption, input, output));
    }
        
    public List<EncryptionSession> GetAllSessions()
    {
        var sessions = new List<EncryptionSession>(_archivedSessions)
        {
            _currentSession.Clone() 
        };
        return sessions;
    }
}