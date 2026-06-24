using CipherLib.Service;

namespace CipherLib.Practical;

public sealed class ClassicalCipherStrategy : ITextCipherStrategy
{
    private readonly CipherWorkflow _workflow;

    public ClassicalCipherStrategy(string displayName, CipherWorkflowOptions options)
    {
        DisplayName = displayName;
        _workflow = new CipherWorkflow(options);
    }

    public string DisplayName { get; }
    public string LastDetails => _workflow.LastAlphabetVariant;

    public string Encrypt(string input)
    {
        return _workflow.Encrypt(input);
    }

    public string Decrypt(string input)
    {
        return _workflow.Decrypt(input);
    }
}
