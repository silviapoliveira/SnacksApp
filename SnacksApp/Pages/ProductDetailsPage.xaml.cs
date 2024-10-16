using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class ProductDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _productId;
    private bool _loginPageDisplayed = false;
    private readonly FavoriteService _favoriteService;
    private string? _imageUrl;

    public ProductDetailsPage(int productId, string productName, ApiService apiService, IValidator validator, FavoriteService favoriteService)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _productId = productId;
        Title = productName ?? "Product Details";
        _favoriteService = favoriteService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductDetails(_productId);
        UpdateFavoriteButton();
    }
    private async Task<Product?> GetProductDetails(int produtoId)
    {
        var (productDetail, errorMessage) = await _apiService.GetProductDetails(produtoId);

        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        if (productDetail == null)
        {
            // Handle error, display message or log in
            await DisplayAlert("Error", errorMessage ?? "Unable to retrieve product.", "OK");
            return null;
        }

        if (productDetail != null)
        {
            // Updating control properties with product data
            ProductImage.Source = productDetail.ImagePath;
            LblProductName.Text = productDetail.Name;
            LblProductPrice.Text = productDetail.Price.ToString();
            LblProductDescription.Text = productDetail.Details;
            LblTotalPrice.Text = productDetail.Price.ToString();
            _imageUrl = productDetail.ImagePath;
        }
        else
        {
            await DisplayAlert("Error", errorMessage ?? "Unable to retrieve product details.", "OK");
            return null;
        }
        return productDetail;
    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator, _favoriteService));
    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantity.Text, out int quantity) &&
           decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            // Decreases the quantity, and does not allow it to be less than 1
            quantity = Math.Max(1, quantity - 1);
            LblQuantity.Text = quantity.ToString();

            // Calculate total price
            var totalPrice = quantity * unitPrice;
            LblTotalPrice.Text = totalPrice.ToString(); // Format as currency
        }
        else
        {
            // Handle if conversions fail
            DisplayAlert("Error", "Invalid values", "OK");
        }
    }

    private void BtnAdd_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantity.Text, out int quantity) &&
             decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            // Increase the quantity
            quantity++;
            LblQuantity.Text = quantity.ToString();

            // Calculate total price
            var totalPrice = quantity * unitPrice;
            LblTotalPrice.Text = totalPrice.ToString(); // Format as currency
        }
        else
        {
            // Handle if conversions fail
            DisplayAlert("Error", "Invalid values", "OK");
        }
    }

    private async void BtnAddToCart_Clicked(object sender, EventArgs e)
    {
        try
        {
            var shoppingCart = new ShoppingCart()
            {
                Quantity = Convert.ToInt32(LblQuantity.Text),
                UnitPrice = Convert.ToDecimal(LblProductPrice.Text),
                TotalValue = Convert.ToDecimal(LblTotalPrice.Text),
                ProductId = _productId,
                ClientId = Preferences.Get("userid", 0)
            };
            var response = await _apiService.AddItemShoppingCart(shoppingCart);
            if (response.Data)
            {
                await DisplayAlert("Success", "Item added to cart!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", $"Failed to add item: {response.ErrorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void BtnFavoriteImg_Clicked(object sender, EventArgs e)
    {
        try
        {
            var existsFavorite = await _favoriteService.ReadAsync(_productId);
            if (existsFavorite is not null)
            {
                await _favoriteService.DeleteAsync(existsFavorite);
            }
            else
            {
                var favoriteProduct = new FavoriteProduct()
                {
                    ProductId = _productId,
                    IsFavorite = true,
                    Details = LblProductDescription.Text,
                    Name = LblProductName.Text,
                    Price = Convert.ToDecimal(LblProductPrice.Text),
                    ImageUrl = _imageUrl
                };

                await _favoriteService.CreateAsync(favoriteProduct);
            }

            UpdateFavoriteButton();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async void UpdateFavoriteButton()
    {
        var existeFavorito = await
               _favoriteService.ReadAsync(_productId);

        if (existeFavorito is not null)
            BtnFavoriteImg.Source = "heartfill";
        else
            BtnFavoriteImg.Source = "heart";
    }
}