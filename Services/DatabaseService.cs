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

        public DatabaseService()
        {
            Init();
        }

        private async void Init()
        {
            if (_database != null)
                return;

            // Get the database path
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "CarRepairShop.db");

            // Create the connection
            _database = new SQLiteAsyncConnection(databasePath);

            // Create all tables
            await _database.CreateTableAsync<Customer>();
            await _database.CreateTableAsync<Car>();
            await _database.CreateTableAsync<RepairTask>();
            await _database.CreateTableAsync<CompletedWork>();
            await _database.CreateTableAsync<Material>();
        }

        // Customer methods
        public async Task<int> SaveCustomerAsync(Customer customer)
        {
            if (customer.Id != 0)
                return await _database.UpdateAsync(customer);
            else
                return await _database.InsertAsync(customer);
        }

        public async Task<Customer> GetCustomerAsync(int id)
        {
            return await _database.Table<Customer>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _database.Table<Customer>().ToListAsync();
        }

        // Car methods
        public async Task<int> SaveCarAsync(Car car)
        {
            if (car.Id != 0)
                return await _database.UpdateAsync(car);
            else
                return await _database.InsertAsync(car);
        }

        public async Task<Car> GetCarAsync(int id)
        {
            return await _database.Table<Car>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Car>> GetCarsByCustomerAsync(int customerId)
        {
            return await _database.Table<Car>().Where(c => c.CustomerId == customerId).ToListAsync();
        }

        // Task methods
        public async Task<int> SaveTaskAsync(RepairTask task)
        {
            if (task.Id != 0)
                return await _database.UpdateAsync(task);
            else
                return await _database.InsertAsync(task);
        }

        public async Task<RepairTask> GetTaskAsync(int id)
        {
            return await _database.Table<RepairTask>().Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<RepairTask>> GetTasksForDateAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _database.Table<RepairTask>()
                .Where(t => t.ScheduledDateTime >= startDate && t.ScheduledDateTime < endDate)
                .ToListAsync();
        }

        public async Task<List<TaskDisplayModel>> GetTaskDisplayModelsForDateAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var tasks = await _database.Table<RepairTask>()
                .Where(t => t.ScheduledDateTime >= startDate && t.ScheduledDateTime < endDate)
                .ToListAsync();

            var displayModels = new List<TaskDisplayModel>();

            foreach (var task in tasks)
            {
                // Get the car associated with this specific task
                var car = await GetCarAsync(task.CarId);

                if (car != null)
                {
                    // Get the customer associated with this specific car
                    var customer = await GetCustomerAsync(car.CustomerId);

                    if (customer != null)
                    {
                        displayModels.Add(new TaskDisplayModel
                        {
                            Id = task.Id,
                            CustomerName = customer.Name ?? "Unknown Customer",
                            CarInfo = $"{car.Make} {car.Model}",
                            RegistrationNumber = car.RegistrationNumber,
                            ScheduledDateTime = task.ScheduledDateTime,
                            Description = task.Description,
                            Status = task.Status
                        });
                    }
                }
            }

            return displayModels;
        }

        // CompletedWork methods
        public async Task<int> SaveCompletedWorkAsync(CompletedWork work)
        {
            if (work.Id != 0)
                return await _database.UpdateAsync(work);
            else
                return await _database.InsertAsync(work);
        }

        public async Task<CompletedWork> GetCompletedWorkByTaskAsync(int taskId)
        {
            return await _database.Table<CompletedWork>().Where(w => w.TaskId == taskId).FirstOrDefaultAsync();
        }

        // Material methods
        public async Task<int> SaveMaterialAsync(Material material)
        {
            if (material.Id != 0)
                return await _database.UpdateAsync(material);
            else
                return await _database.InsertAsync(material);
        }

        public async Task<List<Material>> GetMaterialsByCompletedWorkAsync(int completedWorkId)
        {
            return await _database.Table<Material>().Where(m => m.CompletedWorkId == completedWorkId).ToListAsync();
        }
    }
}