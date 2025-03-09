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
                var customer = new Customer
                {
                    Name = CustomerName,
                    Address = CustomerAddress
                };

                // Save customer and get the newly assigned ID
                var customerId = await _databaseService.SaveCustomerAsync(customer);

                // Verify customer was saved properly
                System.Diagnostics.Debug.WriteLine($"Created customer with ID: {customerId}, Name: {CustomerName}");

                // Get the saved customer to verify
                var savedCustomer = await _databaseService.GetCustomerAsync(customerId);
                System.Diagnostics.Debug.WriteLine($"Retrieved customer with ID: {customerId}, Name: {savedCustomer?.Name}");

                // Create and save car
                var car = new Car
                {
                    CustomerId = customerId, 
                    Make = CarMake,
                    Model = CarModel,
                    RegistrationNumber = RegistrationNumber
                };

                var carId = await _databaseService.SaveCarAsync(car);
                System.Diagnostics.Debug.WriteLine($"Created car with ID: {carId}, Make: {CarMake}, Model: {CarModel}, Reg: {RegistrationNumber}");

                // Create and save task
                var scheduledDateTime = ScheduledDate.Date.Add(ScheduledTime);
                var repairTask = new RepairTask
                {
                    CarId = carId, 
                    Description = TaskDescription,
                    ScheduledDateTime = scheduledDateTime,
                    Status = "Scheduled"
                };

                var taskId = await _databaseService.SaveTaskAsync(repairTask);
                System.Diagnostics.Debug.WriteLine($"Created task with ID: {taskId}, Description: {TaskDescription}, DateTime: {scheduledDateTime}");

                // Clear form fields
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
                System.Diagnostics.Debug.WriteLine($"Error in BookTask: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Exception details: {ex}");
                BookingMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}