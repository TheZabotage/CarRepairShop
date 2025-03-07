using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepairShop.Models
{
    public class RepairTask
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int CarId { get; set; }
        public string Description { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string Status { get; set; } = "Scheduled";
    }
}
