namespace SnacksApp.Pages;

public partial class ConfirmedOrderPage : ContentPage
{
	public ConfirmedOrderPage()
	{
		InitializeComponent();
	}

    private async void BtnBack_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}