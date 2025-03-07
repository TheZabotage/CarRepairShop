using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepairShop.Models
{
    public class Car
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int CustomerId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string RegistrationNumber { get; set; }
    }
}
