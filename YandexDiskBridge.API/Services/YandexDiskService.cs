using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Web;
using Newtonsoft.Json;
using YandexDiskBridge.API.Helper;
using YandexDiskBridge.API.Models;

namespace YandexDiskBridge.API.Services;

public class YandexDiskService : IYandexDiskService
{
    private const string OAuthToken = "y0_AgAAAABD1-P9AADLWwAAAADuD092CtqzcrAhTue9Dcp_DALBunU3Hjw";
    private const string BaseUrl = "https://cloud-api.yandex.net/v1/disk/";
    
    private readonly ILogger<YandexDiskService> _logger;
    
    public YandexDiskService(ILogger<YandexDiskService> logger)
    {
        _logger = logger;
    }


    public async Task<OperationResult> GetInfoByToken()
    {
        try
        {
            var uri = new Uri(BaseUrl);

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {OAuthToken}");
            
            var response = await httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                return SendData(content);
            }

            throw new HttpRequestException($"GetInfoByToken: Ошибка при выполнении запроса: {response.StatusCode}");
        }
        catch (Exception e)
        {
            return HandleError(e);
        }
    }

    public async Task<OperationResult> GetPhotoUrls(string request)
    {
        try
        {
            RequestValidator(request);

            const string url = BaseUrl + "public/resources";

            var uri = AddKeyToUri(url, request, "public_key");

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {OAuthToken}");
            
            var response = await httpClient.GetAsync(uri);

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

    public async Task<OperationResult> GetPhotoByteArray(string request)
    {
        try
        {
            RequestValidator(request);

            const string url = BaseUrl + "public/resources";

            var uri = AddKeyToUri(url, request, "public_key");

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {OAuthToken}");

            var response = await httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var rawJson = await response.Content.ReadAsStringAsync();

                var itemsList = JsonConvert.DeserializeObject<YandexItems.Response>(rawJson);

                if (itemsList is null)
                {
                    throw new InvalidOperationException("GetPhotoByteArray: Ошибка десерелиализации");
                }

                var listResponse = new Photo.ListResponse(
                    itemsList.Embedded.Items.Select(
                        item => new Photo
                        {
                            Title = item.Name,
                            MineType = item.MimeType,
                            PhotoData = GetPhotoBytes(item.File)
                        }
                    ).ToList());

                return SendData(listResponse);
            }

            throw new HttpRequestException($"Ошибка при выполнении запроса: {response.StatusCode}");
        }
        catch (Exception e)
        {
            return HandleError(e);
        }
    }


    public async Task<OperationResult> GetPhotosByteArrayByUrl(string request)
    {
        try
        {
            RequestValidator(request);

            const string url = BaseUrl + "public/resources/download";
            var uri = AddKeyToUri(url, request, "public_key");

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {OAuthToken}");
            var response = await httpClient.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                var archiveUrl = GetUrlForZipArchive(content);
                var zipArchive = await GetZipArchiveByUrl(archiveUrl);
                var unzipPhotos = await GetPhotoByZipArchive(zipArchive);
                
                var listResponse = unzipPhotos
                    .Select(item => new Photo
                        {
                            Title = item.Title,
                            MineType = item.MineType,
                            PhotoData = item.PhotoData
                        }).ToList();

                return SendData(listResponse);
            }

            throw new HttpRequestException($"Error: {response.StatusCode}");

        }
        catch (Exception e)
        {
            return HandleError(e);
        }
    }


    private async Task<byte[]> GetZipArchiveByUrl(string url)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error while get zip archive. Code: {response.StatusCode}");
        }
        
        return await response.Content.ReadAsByteArrayAsync();
    }
    
    private async Task<List<Photo>> GetPhotoByZipArchive(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        var foundedPhotos = new List<Photo>();
        
        foreach (var entry in archive.Entries)
        {
            if (!entry.FullName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                !entry.FullName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) &&
                !entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                continue;

            await using var imageStream = entry.Open();

            using var memoryStream = new MemoryStream();

            await  imageStream.CopyToAsync(memoryStream);
            
            foundedPhotos.Add(new Photo
            {
                Title = entry.Name,
                MineType = Path.GetExtension(entry.FullName),
                PhotoData = memoryStream.ToArray()
            });
        }

        return foundedPhotos;
    }

    private string GetUrlForZipArchive(string content)
    {
        var json = JsonConvert.DeserializeObject<HrefResponse>(content);

        if (!string.IsNullOrEmpty(json?.Href))
        {
            return json.Href;
        }

        throw new Exception("Link for download the zip archive is null");
    }
    
    
    private byte[] GetPhotoBytes(string photoUrl)
    {
        var photoResponse = _httpClient.GetAsync(photoUrl).Result;

        if (photoResponse.IsSuccessStatusCode)
        {
            return photoResponse.Content.ReadAsByteArrayAsync().Result;
        }

        throw new HttpRequestException($"Ошибка при выполнении запроса для изображения: {photoResponse.StatusCode}");
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

    private static void RequestValidator(string request)
    {
        if (string.IsNullOrEmpty(request))
        {
            throw new ArgumentException("Отсутствует ключ или публичный URL ресурса.");
        }

        if (!request.Contains("disk.yandex.ru"))
        {
            throw new ArgumentException("Запрошенный ресурс не соответсвует disk.yandex.ru");
        }
    }


    private string AddKeyToUri(string url, string? param, string? key)
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

            using MemoryStream memoryStream = new MemoryStream(imageData);

            using Image image = Image.FromStream(memoryStream);

            image.Save(imagePath, ImageFormat.Jpeg);

            Console.WriteLine($"Изображение сохранено по пути: {imagePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при преобразовании массива байтов в изображение: {ex.Message}");
        }
    }
}