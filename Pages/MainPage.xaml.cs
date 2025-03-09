using CarRepairShop.Services;

namespace CarRepairShop.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public MainPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }

        private async void OnBookTaskClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///booking");
        }

        private async void OnViewCalendarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///calendar");
        }

        private async void OnTodayTasksClicked(object sender, EventArgs e)
        {
            // Navigate to calendar page and set the date to today
            await Shell.Current.GoToAsync("///calendar");

            // You would ideally want to pass the date parameter to the calendar page
            // This would require a more sophisticated approach with messaging center or similar
            // For now, the calendar page will default to today's date anyway
        }

        private async void OnPurgeDatabaseClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm Purge",
                "This will delete ALL data in the database. This action cannot be undone. Are you sure?",
                "Yes, Purge Database", "Cancel");

            if (confirm)
            {
                await _databaseService.PurgeDatabase();
                await DisplayAlert("Database Purged", "The database has been reset successfully.", "OK");
            }
        }
    }
}