using Microsoft.AspNetCore.Mvc;
using YandexDiskBridge.API.Services;

namespace YandexDiskBridge.API.Controllers;

public class YandexDiskController : AbstractController
{
    public YandexDiskController(ILogger<AbstractController> logger, IYandexDiskService yandexDiskService) : base(logger, yandexDiskService) { }
    
    
    [HttpGet]
    public async Task<IActionResult> GetInfoByToken()
    {
        var result = await YandexDiskService.GetInfoByToken();
        
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> GetPhotoUrls(string request)
    {
        var result = await YandexDiskService.GetPhotoUrls(request);
        
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> GetPhotosByteArray(string request)
    {
        var result = await YandexDiskService.GetPhotoByteArray(request);
        
        return Ok(result);
    }
}