﻿using YandexDiskBridge.API.Helper;

namespace YandexDiskBridge.API.Services.Base;

public interface IYandexDiskService
{
    Task<OperationResult> GetInfoByToken();
    
    Task<OperationResult> GetPhotoUrls(string request);
    
    Task<OperationResult> GetPhotoByteArray(string request);
    Task<OperationResult> GetPhotosByteArrayByUrl(string url);
}