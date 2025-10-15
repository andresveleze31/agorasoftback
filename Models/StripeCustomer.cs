using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class StripeCustomer
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public int OrganizationId { get; set; }

    }
}