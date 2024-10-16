using SnacksApp.Pages;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private readonly FavoriteService _favoriteService;

        public App(ApiService apiService, IValidator validator, FavoriteService favoriteService)
        {
            InitializeComponent();


            _apiService = apiService;
            _validator = validator;
            _favoriteService = favoriteService;

            SetMainPage();
        }

        private void SetMainPage()
        {
            var accessToken = Preferences.Get("accesstoken", string.Empty);

            if (string.IsNullOrEmpty(accessToken))
            {
                MainPage = new NavigationPage(new LoginPage(_apiService, _validator, _favoriteService));
                return;
            }

            MainPage = new AppShell(_apiService, _validator, _favoriteService);
        }
    }
}
