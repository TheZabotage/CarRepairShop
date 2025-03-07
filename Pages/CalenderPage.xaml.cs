using CarRepairShop.ViewModels;

namespace CarRepairShop.Pages
{
    public partial class CalendarPage : ContentPage
    {
        private CalendarViewModel _viewModel;

        public CalendarPage(CalendarViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadTasksForDateCommand.Execute(null);
        }

        private async void OnReturnToMainPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///main");
        }
    }
}