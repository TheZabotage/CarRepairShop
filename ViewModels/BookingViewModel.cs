using CarRepairShop.Models;
using CarRepairShop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CarRepairShop.ViewModels
{
    public partial class BookingViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        // Customer Properties
        [ObservableProperty]
        private string _customerName;

        [ObservableProperty]
        private string _customerAddress;

        // Car Properties
        [ObservableProperty]
        private string _carMake;

        [ObservableProperty]
        private string _carModel;

        [ObservableProperty]
        private string _registrationNumber;

        // Task Properties
        [ObservableProperty]
        private DateTime _scheduledDate = DateTime.Today;

        [ObservableProperty]
        private TimeSpan _scheduledTime = new TimeSpan(8, 0, 0); // Default

        [ObservableProperty]
        private string _taskDescription;

        [ObservableProperty]
        private string _bookingMessage;

        public BookingViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Book New Task";
        }

        [RelayCommand]
        public async Task BookTask()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(CustomerName) ||
                string.IsNullOrWhiteSpace(CustomerAddress) ||
                string.IsNullOrWhiteSpace(CarMake) ||
                string.IsNullOrWhiteSpace(CarModel) ||
                string.IsNullOrWhiteSpace(RegistrationNumber) ||
                string.IsNullOrWhiteSpace(TaskDescription))
            {
                BookingMessage = "Please fill in all fields";
                return;
            }

            IsBusy = true;

            try
            {
                // Create and save customer
                var customer = new Customer
                {
                    Name = CustomerName,
                    Address = CustomerAddress
                };
                var customerId = await _databaseService.SaveCustomerAsync(customer);

                // Create and save car
                var car = new Car
                {
                    CustomerId = customerId,
                    Make = CarMake,
                    Model = CarModel,
                    RegistrationNumber = RegistrationNumber
                };
                var carId = await _databaseService.SaveCarAsync(car);

                // Create and save task
                var scheduledDateTime = ScheduledDate.Date.Add(ScheduledTime);
                var repairTask = new RepairTask
                {
                    CarId = carId,
                    Description = TaskDescription,
                    ScheduledDateTime = scheduledDateTime,
                    Status = "Scheduled"
                };
                await _databaseService.SaveTaskAsync(repairTask);

                // Clear form
                CustomerName = string.Empty;
                CustomerAddress = string.Empty;
                CarMake = string.Empty;
                CarModel = string.Empty;
                RegistrationNumber = string.Empty;
                TaskDescription = string.Empty;
                ScheduledDate = DateTime.Today;
                ScheduledTime = new TimeSpan(8, 0, 0);

                BookingMessage = "Booking successfully saved!";
            }
            catch (Exception ex)
            {
                BookingMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}