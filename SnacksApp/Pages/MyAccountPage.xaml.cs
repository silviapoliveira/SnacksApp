using SnacksApp.Services;

namespace SnacksApp.Pages;

public partial class MyAccountPage : ContentPage
{
    private readonly ApiService _apiService;

    private const string UserNameKey = "username";
    private const string UserEmailKey = "useremail";
    private const string UserPhoneKey = "userphone";

    public MyAccountPage(ApiService apiService)
	{
		InitializeComponent();

        _apiService = apiService;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LoadUserData();
        BtnProfileImg.Source = await GetProfileImageAsync();
    }

    private void LoadUserData()
    {
        LblUserName.Text = Preferences.Get(UserNameKey, string.Empty);
        EntName.Text = LblUserName.Text;
        EntEmail.Text = Preferences.Get(UserEmailKey, string.Empty);
        EntPhone.Text = Preferences.Get(UserPhoneKey, string.Empty);
    }

    private async Task<string?> GetProfileImageAsync()
    {
        string defaultImage = AppConfig.DefaultProfileImage;

        var (response, errorMessage) = await _apiService.GetUserProfileImage();

        if (errorMessage != null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Error", "Not authorized", "OK");
                    return defaultImage;
                default:
                    await DisplayAlert("Erroe", errorMessage ?? "Unable to retrieve image.", "OK");
                    return defaultImage;
            }
        }

        if (response?.UrlImage != null)
        {
            return response.ImagePath;
        }
        return defaultImage;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        // Saves data changed by the user in preferences
        Preferences.Set(UserNameKey, EntName.Text);
        Preferences.Set(UserEmailKey, EntEmail.Text);
        Preferences.Set(UserPhoneKey, EntPhone.Text);
        await DisplayAlert("Data Saved", "Your data has been saved successfully!", "OK");
    }
}