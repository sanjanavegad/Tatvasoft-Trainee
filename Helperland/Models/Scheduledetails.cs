using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helperland.Models
{
    public class Scheduledetails
    {
        public DateTime ServiceStartDate { get; set; }
        public double ServiceHours { get; set; }
        public bool HasPets { get; set; }
    }
}
