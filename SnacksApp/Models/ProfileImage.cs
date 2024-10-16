namespace SnacksApp.Models
{
    public class ProfileImage
    {
        public string? UrlImage { get; set; }

        public string? ImagePath => AppConfig.BaseUrl + UrlImage;
    }
}
