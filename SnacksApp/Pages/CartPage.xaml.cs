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

        bool savedAddress = Preferences.ContainsKey("address");

        if (savedAddress)
        {
            string name = Preferences.Get("name", string.Empty);
            string address = Preferences.Get("address", string.Empty);
            string phonenumber = Preferences.Get("phonenumber", string.Empty);

            // Formatar os dados conforme desejado na label
            LblAddress.Text = $"{name}\n{address}\n{phonenumber}";
        } else
        {
            LblAddress.Text = "Please insert your address";
        }
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
}