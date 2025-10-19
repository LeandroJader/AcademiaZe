using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using System.Text.RegularExpressions;
namespace AcademiaDoZe.Presentation.AppMaui.Views;
public partial class AlunoPage : ContentPage
{
    public AlunoPage(AlunoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AlunoViewModel viewModel)

        {
            await viewModel.InitializeAsync();
        }
    }
    private void OnShowPasswordToggled(object? sender, ToggledEventArgs e)
    {
        if (SenhaEntry is not null)
        {
            // Switch.IsToggled == true -> mostrar senha -> IsPassword = false
            SenhaEntry.IsPassword = !e.Value;
        }
    }
        // Validação simples de e-mail ao perder foco
    private void OnEmailUnfocused(object? sender, FocusEventArgs e)
    {
            var email = EmailEntry?.Text?.Trim();
            // se vazio, limpa a mensagem
            if (string.IsNullOrEmpty(email))
            {
                EmailErrorLabel.IsVisible = false;
                return;
            }
            // regex simples: local@dominio.tld
            var re = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!re.IsMatch(email))
            {
                EmailErrorLabel.IsVisible = true;
                EmailErrorLabel.Text = "Formato de e-mail inválido. Use nome@dominio.com";
                EmailEntry?.Focus();
            }
            else { EmailErrorLabel.IsVisible = false; }
    }
    

}