namespace SnacksApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? UrlImage { get; set; }

        public string? ImagePath => AppConfig.BaseUrl + UrlImage;
    }
}
