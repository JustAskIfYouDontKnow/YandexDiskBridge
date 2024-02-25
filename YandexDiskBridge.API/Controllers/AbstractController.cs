using Microsoft.AspNetCore.Mvc;
using YandexDiskBridge.API.Services;
using YandexDiskBridge.API.Services.Base;

namespace YandexDiskBridge.API.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Produces("application/json")]
[Route("api/[controller]/[action]")]

public class AbstractController : ControllerBase
{
    public readonly ILogger<AbstractController> Logger;
    public readonly IYandexDiskService YandexDiskService;

    public AbstractController(ILogger<AbstractController> logger, IYandexDiskService yandexDiskService)
    {
        Logger = logger;
        YandexDiskService = yandexDiskService;
    }
}