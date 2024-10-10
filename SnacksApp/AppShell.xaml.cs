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
            var carrinhoPage = new CartPage();
            var favoritosPage = new PopularPage();
            var perfilPage = new ProfilePage();

            Items.Add(new TabBar
            {
                Items =
            {
                new ShellContent { Title = "Home",Icon = "home",Content = homePage },
                new ShellContent { Title = "Carrinho", Icon = "cart",Content = carrinhoPage },
                new ShellContent { Title = "Favoritos",Icon = "heart",Content = favoritosPage },
                new ShellContent { Title = "Perfil",Icon = "profile",Content = perfilPage }
            }
            });
        }
    }
}
