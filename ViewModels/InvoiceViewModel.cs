using CarRepairShop.Models;
using CarRepairShop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CarRepairShop.ViewModels
{
    [QueryProperty(nameof(TaskId), "taskId")]
    public partial class InvoiceViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int _taskId;

        [ObservableProperty]
        private TaskDisplayModel _taskInfo;

        [ObservableProperty]
        private string _mechanicName;

        [ObservableProperty]
        private double _hoursWorked;

        [ObservableProperty]
        private double _hourlyRate = 100.0; // Default hourly rate

        [ObservableProperty]
        private string _materialDescription;

        [ObservableProperty]
        private double _materialPrice;

        [ObservableProperty]
        private int _materialQuantity = 1;

        [ObservableProperty]
        private ObservableCollection<Material> _materials = new ObservableCollection<Material>();

        [ObservableProperty]
        private double _totalCost;

        [ObservableProperty]
        private string _saveMessage;

        public InvoiceViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Invoice";
        }

        [RelayCommand]
        public async Task AddMaterial()
        {
            if (string.IsNullOrWhiteSpace(MaterialDescription) || MaterialPrice <= 0 || MaterialQuantity <= 0)
            {
                SaveMessage = "Please enter valid material details";
                return;
            }

            var material = new Material
            {
                Description = MaterialDescription,
                Price = MaterialPrice,
                Quantity = MaterialQuantity
            };

            Materials.Add(material);

            // Clear material input fields
            MaterialDescription = string.Empty;
            MaterialPrice = 0;
            MaterialQuantity = 1;

            CalculateTotalCost();
        }

        [RelayCommand]
        public void RemoveMaterial(Material material)
        {
            if (material != null)
            {
                Materials.Remove(material);
                CalculateTotalCost();
            }
        }

        private void CalculateTotalCost()
        {
            double laborCost = HoursWorked * HourlyRate;
            double materialsCost = 0;

            foreach (var material in Materials)
            {
                materialsCost += material.Price * material.Quantity;
            }

            TotalCost = laborCost + materialsCost;
        }

        partial void OnHoursWorkedChanged(double value)
        {
            CalculateTotalCost();
        }

        partial void OnHourlyRateChanged(double value)
        {
            CalculateTotalCost();
        }

        [RelayCommand]
        public async Task LoadTask()
        {
            if (TaskId <= 0)
                return;

            IsBusy = true;

            try
            {
                var task = await _databaseService.GetTaskAsync(TaskId);
                var car = await _databaseService.GetCarAsync(task.CarId);
                var customer = await _databaseService.GetCustomerAsync(car.CustomerId);

                TaskInfo = new TaskDisplayModel
                {
                    Id = task.Id,
                    CustomerName = customer.Name,
                    CarInfo = $"{car.Make} {car.Model}",
                    RegistrationNumber = car.RegistrationNumber,
                    ScheduledDateTime = task.ScheduledDateTime,
                    Description = task.Description,
                    Status = task.Status
                };

                // Check if work has already been completed for this task
                var completedWork = await _databaseService.GetCompletedWorkByTaskAsync(TaskId);
                if (completedWork != null)
                {
                    MechanicName = completedWork.MechanicName;
                    HoursWorked = completedWork.HoursWorked;
                    HourlyRate = completedWork.HourlyRate;

                    var materials = await _databaseService.GetMaterialsByCompletedWorkAsync(completedWork.Id);
                    Materials.Clear();
                    foreach (var material in materials)
                    {
                        Materials.Add(material);
                    }

                    TotalCost = completedWork.TotalCost;
                }
            }
            catch (Exception)
            {
                // Handle error
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SaveInvoice()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(MechanicName) || HoursWorked <= 0 || HourlyRate <= 0)
            {
                SaveMessage = "Please enter mechanic name, hours worked, and hourly rate";
                return;
            }

            IsBusy = true;

            try
            {
                // Update task status
                var task = await _databaseService.GetTaskAsync(TaskId);
                task.Status = "Completed";
                await _databaseService.SaveTaskAsync(task);

                // Save completed work
                var completedWork = await _databaseService.GetCompletedWorkByTaskAsync(TaskId);

                if (completedWork == null)
                {
                    completedWork = new CompletedWork
                    {
                        TaskId = TaskId,
                        MechanicName = MechanicName,
                        HoursWorked = HoursWorked,
                        HourlyRate = HourlyRate,
                        TotalCost = TotalCost
                    };
                }
                else
                {
                    completedWork.MechanicName = MechanicName;
                    completedWork.HoursWorked = HoursWorked;
                    completedWork.HourlyRate = HourlyRate;
                    completedWork.TotalCost = TotalCost;
                }

                var completedWorkId = await _databaseService.SaveCompletedWorkAsync(completedWork);

                // Save materials
                foreach (var material in Materials)
                {
                    if (material.Id == 0)
                    {
                        material.CompletedWorkId = completedWorkId;
                        await _databaseService.SaveMaterialAsync(material);
                    }
                }

                SaveMessage = "Invoice saved successfully!";
            }
            catch (Exception ex)
            {
                SaveMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnTaskIdChanged(int value)
        {
            if (value > 0)
            {
                LoadTaskCommand.Execute(null);
            }
        }
    }
}