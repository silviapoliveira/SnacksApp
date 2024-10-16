namespace SnacksApp.Pages;

public partial class EmptyCartPage : ContentPage
{
	public EmptyCartPage()
	{
		InitializeComponent();
	}

    private async void BtnBack_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}