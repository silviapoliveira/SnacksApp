using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class OrdersPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoriteService;
    private bool _loginPageDisplayed = false;

    public OrdersPage(ApiService apiService, IValidator validator, FavoriteService favoriteService)
	{
		InitializeComponent();

        _apiService = apiService;
        _validator = validator;
        _favoriteService = favoriteService;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetOrdersList();
    }

    private async Task GetOrdersList()
    {
        try
        {
            // Display the load indicator
            loadOrdersIndicator.IsRunning = true;
            loadOrdersIndicator.IsVisible = true;

            var (orders, errorMessage) = await _apiService.GetOrdersByUser(Preferences.Get("userid", 0));

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }
            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Warning", "There are no orders for this customer.", "OK");
                return;
            }
            if (orders == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Unable to retrieve orders.", "OK");
                return;
            }
            else
            {
                CvOrders.ItemsSource = orders;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "An error occurred while retrieving orders. Try again later.", "OK");
        }
        finally
        {
            // Hide the load indicator
            loadOrdersIndicator.IsRunning = false;
            loadOrdersIndicator.IsVisible = false;
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favoriteService));
    }

    private void CvOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as OrdersByUser;

        if (selectedItem == null) return;

        Navigation.PushAsync(new OrderDetailPage(selectedItem.Id,
                                                    selectedItem.Total,
                                                    _apiService,
                                                    _validator,
                                                    _favoriteService));

        ((CollectionView)sender).SelectedItem = null;
    }
}