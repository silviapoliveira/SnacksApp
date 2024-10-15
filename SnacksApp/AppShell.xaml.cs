using SnacksApp.Pages;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;

        public AppShell(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;
            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService, _validator);
            var cartPage = new CartPage(_apiService, _validator);
            var favoritesPage = new PopularPage();
            var profilePage = new ProfilePage();

            Items.Add(new TabBar
            {
                Items =
            {
                new ShellContent { Title = "Home",Icon = "home",Content = homePage },
                new ShellContent { Title = "Cart", Icon = "cart",Content = cartPage },
                new ShellContent { Title = "Favorites",Icon = "heart",Content = favoritesPage },
                new ShellContent { Title = "Profile",Icon = "profile",Content = profilePage }
            }
            });
        }
    }
}
