using YandexDiskBridge.API.Helper;
using YandexDiskBridge.API.Models;

namespace YandexDiskBridge.API.Services;

public interface IYandexDiskService
{
    Task<OperationResult> GetInfoFromYandex();
    
    Task<OperationResult> GetPhotoUrls(RequestModel requestModel);
    
    Task<OperationResult> GetPhotoByteArray(RequestModel requestModel);
}