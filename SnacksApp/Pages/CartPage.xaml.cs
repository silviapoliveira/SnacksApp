using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;
using System.Collections.ObjectModel;

namespace SnacksApp.Pages;

public partial class CartPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoriteService;
    private bool _loginPageDisplayed = false;
    private bool _isNavigatingToEmptyCartPage = false;

    private ObservableCollection<ShoppingCartItem> ShoppingCartItems = new ObservableCollection<ShoppingCartItem>();

    public CartPage(ApiService apiService, IValidator validator, FavoriteService favoriteService)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _favoriteService = favoriteService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (IsNavigatingToEmptyCartPage()) return;

        bool hasItems = await GetShoppingCartItems();

        if (hasItems)
        {
            DisplayAddress();
        }
        else
        {
            await NavigateToEmptyCart();
        }
    }

    private bool IsNavigatingToEmptyCartPage()
    {
        if (_isNavigatingToEmptyCartPage)
        {
            _isNavigatingToEmptyCartPage = false;
            return true;
        }
        return false;
    }

    private async Task<bool> GetShoppingCartItems()
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
                return false;
            }

            if (shoppingCartItems == null)
            {
                await DisplayAlert("Error", errorMessage ?? "Unable to retrieve items from shopping cart.", "OK");
                return false;
            }

            ShoppingCartItems.Clear();

            foreach (var item in shoppingCartItems)
            {
                ShoppingCartItems.Add(item);
            }

            CvShoppingCart.ItemsSource = ShoppingCartItems;

            UpdateTotalPrice(); // Update total price after updating cart items

            if (!ShoppingCartItems.Any())
            {
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            return false;
        }
    }

    private void DisplayAddress()
    {
        bool savedAddress = Preferences.ContainsKey("address");

        if (savedAddress)
        {
            string name = Preferences.Get("name", string.Empty);
            string address = Preferences.Get("address", string.Empty);
            string phoneNumber = Preferences.Get("phonenumber", string.Empty);

            LblAddress.Text = $"{name}\n{address}\n{phoneNumber}";
        }
        else
        {
            LblAddress.Text = "Please insert your address.";
        }
    }

    private async Task NavigateToEmptyCart()
    {
        LblAddress.Text = string.Empty;
        _isNavigatingToEmptyCartPage = true;
        await Navigation.PushAsync(new EmptyCartPage());
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

        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favoriteService));
    }

    private async void BtnDecrease_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingCartItem itemCart)
        {
            if (itemCart.Quantity == 1) return;
            else
            {
                itemCart.Quantity--;
                UpdateTotalPrice();
                await _apiService.UpdateShoppingCartItemQuantity(itemCart.ProductId, "decrease");
            }
        }
    }

    private async void BtnIncrease_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingCartItem itemCart)
        {
            itemCart.Quantity++;
            UpdateTotalPrice();
            await _apiService.UpdateShoppingCartItemQuantity(itemCart.ProductId, "increase");
        }
    }

    private async void BtnDelete_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is ShoppingCartItem itemCart)
        {
            bool response = await DisplayAlert("Confirm", "Are you sure you want to delete this item from your cart?", "Yes", "No");
            if (response)
            {
                ShoppingCartItems.Remove(itemCart);
                UpdateTotalPrice();
                await _apiService.UpdateShoppingCartItemQuantity(itemCart.ProductId, "delete");
            }
        }
    }

    private void BtnEditAddress_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AddressPage());
    }

    private async void TapConfirmOrder_Tapped(object sender, TappedEventArgs e)
    {
        if (ShoppingCartItems == null || !ShoppingCartItems.Any())
        {
            await DisplayAlert("Information", "Your cart is empty or the order has already been confirmed.", "OK");
            return;
        }

        var order = new Order()
        {
            Address = LblAddress.Text,
            UserId = Preferences.Get("userid", 0),
            Total = Convert.ToDecimal(LblTotalPrice.Text)
        };

        var response = await _apiService.ConfirmOrder(order);

        if (response.HasError)
        {
            if (response.ErrorMessage == "Unauthorized")
            {
                // Redirect to login page
                await DisplayLoginPage();
                return;
            }
            await DisplayAlert("Ups!", $"Something went wrong: {response.ErrorMessage}", "Cancel");
            return;
        }

        ShoppingCartItems.Clear();
        LblAddress.Text = "Please insert your address";
        LblTotalPrice.Text = "0.00";

        await Navigation.PushAsync(new ConfirmedOrderPage());
    }
}