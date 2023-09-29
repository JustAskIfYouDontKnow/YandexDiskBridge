namespace YandexDiskBridge.API.Helper;

public class OperationResult
{
    public object? Data { get; set; }
    public bool IsError { get; set; }
    public string? ErrorMessage { get; set; }
}