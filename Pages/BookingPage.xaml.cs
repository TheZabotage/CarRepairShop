using CarRepairShop.ViewModels;

namespace CarRepairShop.Pages
{
    public partial class BookingPage : ContentPage
    {
        private BookingViewModel _viewModel;

        public BookingPage(BookingViewModel viewModel)
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