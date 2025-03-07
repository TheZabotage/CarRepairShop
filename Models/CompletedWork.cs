using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepairShop.Models
{
    public class CompletedWork
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int TaskId { get; set; }
        public string MechanicName { get; set; }
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public double TotalCost { get; set; }
    }
}
