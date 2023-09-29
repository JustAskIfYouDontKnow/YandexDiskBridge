using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using Newtonsoft.Json;
using YandexDiskBridge.API.Helper;
using YandexDiskBridge.API.Models;

namespace YandexDiskBridge.API.Services;

public class YandexDiskService : IYandexDiskService
{
    private const string OAuthToken = "y0_AgAAAAAmYHzhAADLWwAAAADtw3wD9BcITPpnRTWfffZfPOGh9R0G4EY";
    private readonly HttpClient _httpClient;

    private readonly ILogger<YandexDiskService> _logger;


    public YandexDiskService(HttpClient httpClient, ILogger<YandexDiskService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }


    public async Task<OperationResult> GetInfoFromYandex()
    {
        try
        {
            var apiUrl = new Uri("https://cloud-api.yandex.net/v1/disk");

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {OAuthToken}");

            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return SendData(content);
            }
            
            throw new HttpRequestException($"GetInfoFromYandex: Ошибка при выполнении запроса: {response.StatusCode}");
        }
        catch (Exception e)
        {
            return HandleError(e);
        }
    }


    public async Task<OperationResult> GetPhotoUrls(RequestModel requestModel)
    {
        try
        {
            if (string.IsNullOrEmpty(requestModel?.Query))
            {
                throw new ArgumentException("Отсутствует ключ или публичный URL ресурса.");
            }

            const string url = "https://cloud-api.yandex.net/v1/disk/public/resources";

            var uri = AddKeyToQuery(url, requestModel.Query, "public_key");

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {OAuthToken}");

            var response = await _httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var rawJson = await response.Content.ReadAsStringAsync();

                var itemsList = JsonConvert.DeserializeObject<YandexItems.Response>(rawJson);

                if (itemsList is null)
                {
                    throw new InvalidOperationException("GetPhotoUrls: Ошибка десерелиализации");
                }

                var photosUrl = itemsList.Embedded.Items.Select(x => x.File);
                
                return SendData(photosUrl);
            }

            throw new HttpRequestException($"GetPhotoUrls: Ошибка при выполнении запроса: {response.StatusCode}");
        }
        catch (Exception e)
        {
            return HandleError(e);
        }
    }


    public async Task<OperationResult> GetPhotoByteArray(RequestModel requestModel)
    {
        try
        {
            if (string.IsNullOrEmpty(requestModel?.Query))
            {
                throw new ArgumentException("Отсутствует ключ или публичный URL ресурса.");
            }

            const string url = "https://cloud-api.yandex.net/v1/disk/public/resources";

            var uri = AddKeyToQuery(url, requestModel.Query, "public_key");

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {OAuthToken}");

            var response = await _httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var rawJson = await response.Content.ReadAsStringAsync();

                var itemsList = JsonConvert.DeserializeObject<YandexItems.Response>(rawJson);

                if (itemsList is null)
                {
                    throw new InvalidOperationException("GetPhotoByteArray: Ошибка десерелиализации");
                }

                var photoBytesList = new List<byte[]>();

                foreach (var item in itemsList.Embedded.Items)
                {
                    var photoUrl = item.File;
                    var photoResponse = await _httpClient.GetAsync(photoUrl);

                    if (photoResponse.IsSuccessStatusCode)
                    {
                        var photoBytes = await photoResponse.Content.ReadAsByteArrayAsync();
                        photoBytesList.Add(photoBytes);
                    }
                }

                return SendData(photoBytesList);
            }

            throw new HttpRequestException($"Ошибка при выполнении запроса: {response.StatusCode}");
        }
        catch (Exception e)
        {
            return HandleError(e);
        }
    }


    private OperationResult HandleError(Exception e)
    {
        _logger.LogError(e.Message);
        
        return new OperationResult
        {
            Data = null, 
            ErrorMessage = e.Message, 
            IsError = true
        };
    }


    private OperationResult SendData(object data)
    {
        return new OperationResult
        {
            Data = data, 
            ErrorMessage = null, 
            IsError = false
        };
    }


    private string AddKeyToQuery(string url, string? param, string? key)
    {
        var uriBuilder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query[key] = param;
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }


    private void CheckPhotoByteArray(byte[] imageData)
    {
        try
        {
            string currentDirectory = Environment.CurrentDirectory;

            string saveDirectory = Path.Combine(currentDirectory, "Images");

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            string imagePath = Path.Combine(saveDirectory, Guid.NewGuid().ToString());

            using (MemoryStream memoryStream = new MemoryStream(imageData))
            using (Image image = Image.FromStream(memoryStream))
            {
                image.Save(imagePath, ImageFormat.Jpeg);
            }

            Console.WriteLine($"Изображение сохранено по пути: {imagePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при преобразовании массива байтов в изображение: {ex.Message}");
        }
    }
}