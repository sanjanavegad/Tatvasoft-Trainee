using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Helperland.Models
{
    public partial class Rating
    {
        public int RatingId { get; set; }
        public int ServiceRequestId { get; set; }
        public int RatingFrom { get; set; }
        public int RatingTo { get; set; }
        public decimal Ratings { get; set; }
        public string Comments { get; set; }
        public DateTime RatingDate { get; set; }
        public decimal OnTimeArrival { get; set; }
        public decimal Friendly { get; set; }
        public decimal QualityOfService { get; set; }



        public virtual User RatingFromNavigation { get; set; }
        public virtual User RatingToNavigation { get; set; }
        public virtual ServiceRequest ServiceRequest { get; set; }

        [NotMapped]
        public int? ServiceProviderId { get; set; }
        [NotMapped]
        public string Name { get; set; }
    }
}
