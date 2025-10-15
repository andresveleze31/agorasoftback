using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class StripeModel
    {

        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }

        //public string WebhookSecret { get; set; } = string.Empty; // 👈 Agrega esto


    }
}