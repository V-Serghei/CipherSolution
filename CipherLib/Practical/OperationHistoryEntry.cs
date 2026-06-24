namespace CipherLib.Practical;

public sealed class OperationHistoryEntry
{
    public DateTime Time { get; init; } = DateTime.Now;
    public string Algorithm { get; init; } = "";
    public string Mode { get; init; } = "";
    public string Operation { get; init; } = "";
    public string KeyPreview { get; init; } = "";
    public string Input { get; init; } = "";
    public string Output { get; init; } = "";
    public string AlphabetVariant { get; init; } = "";
    public bool RoundTripOk { get; init; }
}
