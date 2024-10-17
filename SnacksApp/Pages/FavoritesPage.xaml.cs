using SnacksApp.Models;
using SnacksApp.Services;
using SnacksApp.Validations;

namespace SnacksApp.Pages;

public partial class FavoritesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoriteService;

    public FavoritesPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();

        _apiService = apiService;
        _validator = validator;
        _favoriteService = ServiceFactory.CreateFavoriteService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetFavoriteProducts();
    }

    private async Task GetFavoriteProducts()
    {
        try
        {
            var favoriteProducts = await _favoriteService.ReadAllAsync();

            if (favoriteProducts is null || favoriteProducts.Count == 0)
            {
                CvProducts.ItemsSource = null; // clears currrent list
                LblWarning.IsVisible = true; // shows warning
            }
            else
            {
                CvProducts.ItemsSource = favoriteProducts;
                LblWarning.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    private void CvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as FavoriteProduct;

        if (currentSelection == null) return;

        Navigation.PushAsync(new ProductDetailsPage(currentSelection.ProductId,
                                                     currentSelection.Name!,
                                                     _apiService, _validator, _favoriteService));

        ((CollectionView)sender).SelectedItem = null;
    }
}