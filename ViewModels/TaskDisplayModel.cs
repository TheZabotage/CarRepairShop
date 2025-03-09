using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRepairShop.ViewModels
{
    public class TaskDisplayModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CarInfo { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public DateTime ScheduledDateTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
