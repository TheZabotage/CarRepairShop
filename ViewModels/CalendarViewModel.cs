using CarRepairShop.Models;
using CarRepairShop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CarRepairShop.ViewModels
{
    public partial class CalendarViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<TaskDisplayModel> _tasks = new ObservableCollection<TaskDisplayModel>();

        [ObservableProperty]
        private TaskDisplayModel _selectedTask;

        public CalendarViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Calendar";
        }

        [RelayCommand]
        public async Task LoadTasksForDate()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Tasks.Clear();
                var tasks = await _databaseService.GetTaskDisplayModelsForDateAsync(SelectedDate);

                foreach (var task in tasks)
                {
                    Tasks.Add(task);
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"Error loading tasks: {ex.Message}");
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

            await Shell.Current.GoToAsync($"invoice?taskId={task.Id}");
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            LoadTasksForDateCommand.Execute(null);
        }
    }
}