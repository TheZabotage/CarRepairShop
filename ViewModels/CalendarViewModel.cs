using CarRepairShop.Models;
using CarRepairShop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CarRepairShop.ViewModels
{
    public partial class CalendarViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<TaskDisplayModel> _tasks;

        [ObservableProperty]
        private TaskDisplayModel _selectedTask;

        [ObservableProperty]
        private bool _hasNoTasks;

        [ObservableProperty]
        private string _statusMessage;

        public CalendarViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Calendar";
            Tasks = new ObservableCollection<TaskDisplayModel>();
        }

        [RelayCommand]
        public async Task LoadTasksForDate()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            StatusMessage = string.Empty;

            try
            {
                // Clear existing data
                Tasks.Clear();

                // Fetch tasks for the selected date
                var tasks = await _databaseService.GetTaskDisplayModelsForDateAsync(SelectedDate);

                Debug.WriteLine($"Retrieved {tasks.Count} tasks for {SelectedDate.ToShortDateString()}");

                // Add each task to the collection
                foreach (var task in tasks)
                {
                    Debug.WriteLine($"Task: {task.Id}, Customer: {task.CustomerName}, Car: {task.CarInfo}, Reg: {task.RegistrationNumber}");
                    Tasks.Add(task);
                }

                // Update the HasNoTasks property
                HasNoTasks = Tasks.Count == 0;

                if (HasNoTasks)
                {
                    StatusMessage = "No tasks scheduled for this date.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading tasks: {ex.Message}";
                Debug.WriteLine($"Error loading tasks: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ViewInvoice(TaskDisplayModel task)
        {
            if (task == null)
                return;

            SelectedTask = task;

            try
            {
                // Navigate to the invoice page with the task ID
                await Shell.Current.GoToAsync($"invoice?taskId={task.Id}");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error navigating to invoice: {ex.Message}";
                Debug.WriteLine($"Error navigating to invoice: {ex}");
            }
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            // When the date changes, load tasks for the new date
            LoadTasksForDateCommand.Execute(null);
        }
    }
}