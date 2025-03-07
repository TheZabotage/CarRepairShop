using CarRepairShop.ViewModels;

namespace CarRepairShop.Pages
{
    public partial class InvoicePage : ContentPage
    {
        private InvoiceViewModel _viewModel;

        public InvoicePage(InvoiceViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        private async void OnReturnToMainPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///main");
        }
    }
}