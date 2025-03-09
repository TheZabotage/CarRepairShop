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

        // PUrges the database and recreates all tables, debugging purposes
        public async Task PurgeDatabase()
        {
            await EnsureInitializedAsync();

            try
            {
                // Drop all tables
                await _database.DropTableAsync<Material>();
                await _database.DropTableAsync<CompletedWork>();
                await _database.DropTableAsync<RepairTask>();
                await _database.DropTableAsync<Car>();
                await _database.DropTableAsync<Customer>();

                // Recreate tables
                await _database.CreateTableAsync<Customer>();
                await _database.CreateTableAsync<Car>();
                await _database.CreateTableAsync<RepairTask>();
                await _database.CreateTableAsync<CompletedWork>();
                await _database.CreateTableAsync<Material>();

                System.Diagnostics.Debug.WriteLine("Database purged successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error purging database: {ex.Message}");
            }
        }


        // Customer methods
        public async Task<int> SaveCustomerAsync(Customer customer)
        {
            await EnsureInitializedAsync();

            // Make sure the customer object isn't null
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
            }

            try
            {
                if (customer.Id != 0)
                {
                    // This is an update to an existing customer
                    await _database.UpdateAsync(customer);
                    System.Diagnostics.Debug.WriteLine($"Updated customer with ID: {customer.Id}, Name: {customer.Name}");
                    return customer.Id;
                }
                else
                {
                    // This is a new customer
                    System.Diagnostics.Debug.WriteLine($"Inserting new customer with Name: {customer.Name}");

                    // Insert the customer
                    await _database.InsertAsync(customer);

                    var lastId = await _database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
                    System.Diagnostics.Debug.WriteLine($"Last inserted row ID: {lastId}");

                    // Update our customer object with this ID
                    customer.Id = lastId;

                    // Double-check that the customer was actually inserted
                    var savedCustomer = await _database.Table<Customer>()
                                                      .Where(c => c.Id == lastId)
                                                      .FirstOrDefaultAsync();

                    if (savedCustomer != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Verified customer saved with ID: {savedCustomer.Id}, Name: {savedCustomer.Name}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"WARNING: Could not verify customer with ID {lastId} was saved!");
                    }

                    return lastId;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving customer: {ex.Message}");
                throw;
            }
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

            // Make sure the car object isn't null
            if (car == null)
            {
                throw new ArgumentNullException(nameof(car), "Car cannot be null");
            }

            try
            {
                if (car.Id != 0)
                {
                    // This is an update to an existing car
                    await _database.UpdateAsync(car);
                    System.Diagnostics.Debug.WriteLine($"Updated car with ID: {car.Id}, Make: {car.Make}, Model: {car.Model}");
                    return car.Id;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Inserting new car with Make: {car.Make}, Model: {car.Model}, CustomerID: {car.CustomerId}");

                    // Insert the car
                    await _database.InsertAsync(car);

                    var lastId = await _database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
                    System.Diagnostics.Debug.WriteLine($"Last inserted row ID: {lastId}");

                    // Update our car object with this ID
                    car.Id = lastId;

                    // Double-check that the car was actually inserted
                    var savedCar = await _database.Table<Car>()
                                                 .Where(c => c.Id == lastId)
                                                 .FirstOrDefaultAsync();

                    if (savedCar != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Verified car saved with ID: {savedCar.Id}, Make: {savedCar.Make}, CustomerID: {savedCar.CustomerId}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"WARNING: Could not verify car with ID {lastId} was saved!");
                    }

                    return lastId;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving car: {ex.Message}");
                throw;
            }
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

            // Make sure the task object isn't null
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task), "Task cannot be null");
            }

            try
            {
                if (task.Id != 0)
                {
                    // This is an update to an existing task
                    await _database.UpdateAsync(task);
                    System.Diagnostics.Debug.WriteLine($"Updated task with ID: {task.Id}, Description: {task.Description}");
                    return task.Id;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Inserting new task with Description: {task.Description}, CarID: {task.CarId}");

                    // Insert the task
                    await _database.InsertAsync(task);

                    var lastId = await _database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
                    System.Diagnostics.Debug.WriteLine($"Last inserted row ID: {lastId}");

                    // Update our task object with this ID
                    task.Id = lastId;

                    // Double-check that the task was actually inserted
                    var savedTask = await _database.Table<RepairTask>()
                                                  .Where(t => t.Id == lastId)
                                                  .FirstOrDefaultAsync();

                    if (savedTask != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Verified task saved with ID: {savedTask.Id}, Description: {savedTask.Description}, CarID: {savedTask.CarId}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"WARNING: Could not verify task with ID {lastId} was saved!");
                    }

                    return lastId;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving task: {ex.Message}");
                throw; // Re-throw to let caller handle it
            }
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
                                RegistrationNumber = car.RegistrationNumber ?? "Unknown",
                                ScheduledDateTime = task.ScheduledDateTime,
                                Description = task.Description ?? "No description",
                                Status = task.Status ?? "Unknown Status"
                            };

                            // Add it to our list
                            displayModels.Add(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing task {task.Id}: {ex.Message}");
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
        //Fire and forget tasks without awaiting them
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