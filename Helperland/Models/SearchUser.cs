using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helperland.Models
{
    public class SearchUser
    {
        public int? ServiceRequestId { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string Mobile { get; set; }

        public int? UserTypeId { get; set; }

        public DateTime? ServiceStartDate { get; set; }

        public string ZipCode { get; set; }

        public int? Status { get; set; }
    }
}
