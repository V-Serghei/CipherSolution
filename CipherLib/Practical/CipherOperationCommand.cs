namespace CipherLib.Practical;

public sealed class CipherOperationCommand
{
    private readonly Func<string> _execute;

    public CipherOperationCommand(string operationName, Func<string> execute)
    {
        OperationName = operationName;
        _execute = execute;
    }

    public string OperationName { get; }

    public string Execute()
    {
        return _execute();
    }
}
