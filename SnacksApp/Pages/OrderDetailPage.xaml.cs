using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class OrderDetailPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoriteService;
    private bool _loginPageDisplayed = false;

    public OrderDetailPage(int orderId,
                              decimal totalPrice,
                              ApiService apiService,
                              IValidator validator,
                              FavoriteService favoriteService)
	{
		InitializeComponent();

        _apiService = apiService;
        _validator = validator;
        _favoriteService = favoriteService;

        LblTotalPrice.Text = " R$" + totalPrice;

        GetOrderDetails(orderId);
    }

    private async void GetOrderDetails(int orderId)
    {
        try
        {
            // Display the load indicator
            loadIndicator.IsRunning = true;
            loadIndicator.IsVisible = true;

            var (orderDetails, errorMessage) = await _apiService.GetOrderDetails(orderId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }

            if (orderDetails is null)
            {
                await DisplayAlert("Error", errorMessage ?? "Unable to retrieve order details.", "OK");
                return;
            }
            else
            {
                CvOrderDetails.ItemsSource = orderDetails;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "An error occurred while retrieving the details. Try again later.", "OK");
        }
        finally
        {
            // Hide the load indicator
            loadIndicator.IsRunning = false;
            loadIndicator.IsVisible = false;
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favoriteService));
    }
}