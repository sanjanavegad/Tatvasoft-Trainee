using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Helperland.Models
{
    public class Completebooking
    {
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Please Enter ServiceDate")]
        [DataType(DataType.Date)]
        public DateTime ServiceDate { get; set; }

        [Required(ErrorMessage = "Please Enter ServiceTime")]
        [DataType(DataType.Time)]
        public DateTime ServiceTime { get; set; }

        [Required(ErrorMessage = "Please Enter ServiceHours")]
        public double ServiceHours { get; set; }

        [Required(ErrorMessage = "Please Enter Zipcode")]
        public string ZipcodeValue { get; set; }

        public decimal? ServiceHourlyRate { get; set; }

        public int ServiceId { get; set; }

        public double ExtraHours { get; set; }

        public double SubTotal { get; set; }

        public double TotalCost { get; set; }

        public string Comments { get; set; }

        public bool HasPets { get; set; }

        public bool Cabinet { get; set; }

        public bool Oven { get; set; }

        public bool Window { get; set; }

        public bool Fridge { get; set; }

        public bool Laundry { get; set; }

        public bool Terms { get; set; }
    }
}
