using Microsoft.AspNetCore.Mvc;
using YandexDiskBridge.API.Helper;
using YandexDiskBridge.API.Models;
using YandexDiskBridge.API.Services;

namespace YandexDiskBridge.API.Controllers;

public class YandexDiskController : AbstractController
{
    public YandexDiskController(ILogger<AbstractController> logger, IYandexDiskService yandexDiskService) : base(logger, yandexDiskService) { }
    
    
    [HttpGet]
    [ProducesResponseType(typeof(SchemaResult<object>), 200)]
    public async Task<IActionResult> GetInfoByToken()
    {
        var result = await YandexDiskService.GetInfoByToken();
        
        return Ok(result);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(SchemaResult<object>), 200)]
    public async Task<IActionResult> GetPhotoUrls(string request)
    {
        var result = await YandexDiskService.GetPhotoUrls(request);
        
        return Ok(result);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(SchemaResult<Photo.ListResponse>), 200)]
    public async Task<IActionResult> GetPhotosByteArray(string request)
    {
        var result = await YandexDiskService.GetPhotoByteArray(request);
        
        return Ok(result);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(SchemaResult<Photo.ListResponse>), 200)]
    public async Task<IActionResult> GetPhotosByteArrayByUrl(string url)
    {
        var result = await YandexDiskService.GetPhotosByteArrayByUrl(url);
        
        return Ok(result);
    }
}

