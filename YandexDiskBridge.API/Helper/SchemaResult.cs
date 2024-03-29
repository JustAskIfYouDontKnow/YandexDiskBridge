﻿namespace YandexDiskBridge.API.Helper;

public class SchemaResult<T>
{
    public T? Data { get; set; }

    public bool IsError { get; set; }
    
    public string? ErrorMessage { get; set; }
}