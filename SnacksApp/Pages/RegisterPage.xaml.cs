using SnacksApp.Services;
using SnacksApp.Validations;
using System.ComponentModel.DataAnnotations;

namespace SnacksApp.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    public RegisterPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void BtnSignup_ClickedAsync(object sender, EventArgs e)
    {
        if (await _validator.Validate(EntName.Text, EntEmail.Text, EntPhone.Text, EntPassword.Text))
        {

            var response = await _apiService.RegisterUser(EntName.Text, EntEmail.Text,
                                                          EntPhone.Text, EntPassword.Text);

            if (!response.HasError)
            {
                await DisplayAlert("Warning", "Your account has been successfully created!", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _validator));
            }
            else
            {
                await DisplayAlert("Error", "Something went wrong!", "Cancel");
            }
        }
        else
        {
            string mensagemErro = "";
            mensagemErro += _validator.NameError != null ? $"\n- {_validator.NameError}" : "";
            mensagemErro += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
            mensagemErro += _validator.PhoneError != null ? $"\n- {_validator.PhoneError}" : "";
            mensagemErro += _validator.PasswordError != null ? $"\n- {_validator.PasswordError}" : "";

            await DisplayAlert("Error", mensagemErro, "OK");
        }
    }

    private async void TapLogin_TappedAsync(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}