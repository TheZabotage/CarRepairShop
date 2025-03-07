using CarRepairShop.Models;
using CarRepairShop.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CarRepairShop.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private bool _initialized = false;

        public DatabaseService()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        private async Task InitializeAsync()
        {
            if (_initialized)
                return;

            // Get the database path
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "CarRepairShop.db");
            // Where is the database stored?

            Console.WriteLine($"DATABASE PATH: {databasePath}");


            // Create the connection
            _database = new SQLiteAsyncConnection(databasePath);

            // Create all tables
            await _database.CreateTableAsync<Customer>();
            await _database.CreateTableAsync<Car>();
            await _database.CreateTableAsync<RepairTask>();
            await _database.CreateTableAsync<CompletedWork>();
            await _database.CreateTableAsync<Material>();

            _initialized = true;
        }

        private async Task EnsureInitializedAsync()
        {
            if (!_initialized)
                await InitializeAsync();
        }

        // Customer methods
        public async Task<int> SaveCustomerAsync(Customer customer)
        {
            await EnsureInitializedAsync();

            if (customer.Id != 0)
                return await _database.UpdateAsync(customer);
            else
                return await _database.InsertAsync(customer);
        }

        public async Task<Customer> GetCustomerAsync(int id)
        {
            await EnsureInitializedAsync();
            return await _database.Table<Customer>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            await EnsureInitializedAsync();
            return await _database.Table<Customer>().ToListAsync();
        }

        // Car methods
        public async Task<int> SaveCarAsync(Car car)
        {
            await EnsureInitializedAsync();

            if (car.Id != 0)
                return await _database.UpdateAsync(car);
            else
                return await _database.InsertAsync(car);
        }

        public async Task<Car> GetCarAsync(int id)
        {
            await EnsureInitializedAsync();
            return await _database.Table<Car>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Car>> GetCarsByCustomerAsync(int customerId)
        {
            await EnsureInitializedAsync();
            return await _database.Table<Car>().Where(c => c.CustomerId == customerId).ToListAsync();
        }

        // Task methods
        public async Task<int> SaveTaskAsync(RepairTask task)
        {
            await EnsureInitializedAsync();

            if (task.Id != 0)
                return await _database.UpdateAsync(task);
            else
                return await _database.InsertAsync(task);
        }

        public async Task<RepairTask> GetTaskAsync(int id)
        {
            await EnsureInitializedAsync();
            return await _database.Table<RepairTask>().Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<RepairTask>> GetTasksForDateAsync(DateTime date)
        {
            await EnsureInitializedAsync();

            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _database.Table<RepairTask>()
                .Where(t => t.ScheduledDateTime >= startDate && t.ScheduledDateTime < endDate)
                .ToListAsync();
        }

        public async Task<List<TaskDisplayModel>> GetTaskDisplayModelsForDateAsync(DateTime date)
        {
            await EnsureInitializedAsync();

            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            // First get all tasks for the date
            var tasks = await _database.Table<RepairTask>()
                .Where(t => t.ScheduledDateTime >= startDate && t.ScheduledDateTime < endDate)
                .ToListAsync();

            // Create a list to hold the display models
            var displayModels = new List<TaskDisplayModel>();

            // Process each task individually
            foreach (var task in tasks)
            {
                try
                {
                    // Get the car for this specific task
                    var car = await _database.Table<Car>().Where(c => c.Id == task.CarId).FirstOrDefaultAsync();

                    if (car != null)
                    {
                        // Get the customer for this specific car
                        var customer = await _database.Table<Customer>().Where(c => c.Id == car.CustomerId).FirstOrDefaultAsync();

                        if (customer != null)
                        {
                            // Create a display model with all the information
                            var model = new TaskDisplayModel
                            {
                                Id = task.Id,
                                CustomerName = customer.Name ?? "Unknown Customer",
                                CarInfo = $"{car.Make} {car.Model}",
                                RegistrationNumber = car.RegistrationNumber,
                                ScheduledDateTime = task.ScheduledDateTime,
                                Description = task.Description,
                                Status = task.Status
                            };

                            // Add it to our list
                            displayModels.Add(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing task {task.Id}: {ex.Message}");
                    // Continue processing other tasks even if one fails
                }
            }

            return displayModels;
        }

        // CompletedWork methods
        public async Task<int> SaveCompletedWorkAsync(CompletedWork work)
        {
            await EnsureInitializedAsync();

            if (work.Id != 0)
                return await _database.UpdateAsync(work);
            else
                return await _database.InsertAsync(work);
        }

        public async Task<CompletedWork> GetCompletedWorkByTaskAsync(int taskId)
        {
            await EnsureInitializedAsync();
            return await _database.Table<CompletedWork>().Where(w => w.TaskId == taskId).FirstOrDefaultAsync();
        }

        // Material methods
        public async Task<int> SaveMaterialAsync(Material material)
        {
            await EnsureInitializedAsync();

            if (material.Id != 0)
                return await _database.UpdateAsync(material);
            else
                return await _database.InsertAsync(material);
        }

        public async Task<List<Material>> GetMaterialsByCompletedWorkAsync(int completedWorkId)
        {
            await EnsureInitializedAsync();
            return await _database.Table<Material>().Where(m => m.CompletedWorkId == completedWorkId).ToListAsync();
        }
    }

    // Helper extension method
    public static class TaskExtensions
    {
        // This method allows us to fire and forget tasks without awaiting them
        // while still handling exceptions
        public static void SafeFireAndForget(this Task task, bool returnToCallingContext, Action<Exception> onException = null)
        {
            task.ContinueWith(t =>
            {
                if (t.IsFaulted && onException != null)
                {
                    onException(t.Exception);
                }
            }, returnToCallingContext ? TaskScheduler.FromCurrentSynchronizationContext() : TaskScheduler.Default);
        }
    }
}