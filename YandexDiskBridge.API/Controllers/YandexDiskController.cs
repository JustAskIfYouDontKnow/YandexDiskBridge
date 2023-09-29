using Microsoft.AspNetCore.Mvc;
using YandexDiskBridge.API.Models;
using YandexDiskBridge.API.Services;

namespace YandexDiskBridge.API.Controllers;

public class YandexDiskController : AbstractController
{
    public YandexDiskController(ILogger<AbstractController> logger, IYandexDiskService yandexDiskService) : base(logger, yandexDiskService) { }
    
    
    [HttpGet]
    public async Task<IActionResult> GetInfo()
    {
        var result = await YandexDiskService.GetInfoFromYandex();
        
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> GetPhotoUrls(RequestModel requestModel)
    {
        var result = await YandexDiskService.GetPhotoUrls(requestModel);
        
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> GetPhotosByteArray(RequestModel requestModel)
    {
        var result = await YandexDiskService.GetPhotoByteArray(requestModel);
        
        return Ok(result);
    }
}