namespace YandexDiskBridge.API.Models;

public class Photo
{
    public string Title { get; set; }
    public string MineType { get; set; }
    public object PhotoData { get; set; }
    
    public class ListResponse
    {
        public ListResponse(List<Photo> photoItems)
        {
            PhotoItems = photoItems;
        }

        public List<Photo> PhotoItems { get; set; }
    }
}