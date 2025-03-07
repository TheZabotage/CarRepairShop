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
        public string CustomerName { get; set; }
        public string CarInfo { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}
