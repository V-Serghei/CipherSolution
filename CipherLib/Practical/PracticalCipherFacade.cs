namespace CipherLib.Practical;

public sealed class PracticalCipherFacade
{
    private readonly List<OperationHistoryEntry> _history = new();

    public IReadOnlyList<OperationHistoryEntry> History => _history;

    public OperationHistoryEntry Execute(
        ITextCipherStrategy strategy,
        CipherOperationCommand command,
        string keyPreview,
        string mode,
        string input)
    {
        string output = command.Execute();
        bool roundTripOk = VerifyRoundTrip(strategy, command.OperationName, input, output);

        var entry = new OperationHistoryEntry
        {
            Algorithm = strategy.DisplayName,
            Mode = mode,
            Operation = command.OperationName,
            KeyPreview = keyPreview,
            Input = input,
            Output = output,
            AlphabetVariant = strategy.LastDetails,
            RoundTripOk = roundTripOk
        };

        _history.Insert(0, entry);
        return entry;
    }

    private static bool VerifyRoundTrip(ITextCipherStrategy strategy, string operation, string input, string output)
    {
        try
        {
            return operation == "Encrypt"
                ? strategy.Decrypt(output) == input
                : strategy.Encrypt(output) == input;
        }
        catch
        {
            return false;
        }
    }
}
