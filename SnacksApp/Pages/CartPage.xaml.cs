using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;
using System.Collections.ObjectModel;

namespace SnacksApp.Pages;

public partial class CartPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    private ObservableCollection<ShoppingCartItem> ShoppingCartItems = new ObservableCollection<ShoppingCartItem>();

    public CartPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetShoppingCartItems();
    }

    private async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems()
    {
        try
        {
            var userId = Preferences.Get("userid", 0);
            var (shoppingCartItems, errorMessage) = await
                     _apiService.GetShoppingCartItems(userId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                // Redirect to login page
                await DisplayLoginPage();
                return Enumerable.Empty<ShoppingCartItem>();
            }

            if (shoppingCartItems == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Unable to retrieve items from shopping cart.", "OK");
                return Enumerable.Empty<ShoppingCartItem>();
            }

            ShoppingCartItems.Clear();

            foreach (var item in shoppingCartItems)
            {
                ShoppingCartItems.Add(item);
            }

            CvShoppingCart.ItemsSource = ShoppingCartItems;

            UpdateTotalPrice(); // Update total price after updating cart items

            
            return shoppingCartItems;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            return Enumerable.Empty<ShoppingCartItem>();
        }
    }

    private void UpdateTotalPrice()
    {
        try
        {
            var totalPrice = ShoppingCartItems.Sum(item => item.Price * item.Quantity);
            LblTotalPrice.Text = totalPrice.ToString();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"An error occurred while updating the total price: {ex.Message}", "OK");
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void BtnDecrease_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnIncrease_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnDelete_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnEditAddress_Clicked(object sender, EventArgs e)
    {

    }
}