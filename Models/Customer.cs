using System;
using SQLite;
using System.Collections.Generic;
namespace CarRepairShop.Models
{
    public class Customer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
    }
}
