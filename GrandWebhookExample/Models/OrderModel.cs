using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandWebhookExample.Models
{
    public class OrderModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public decimal OrderTotal { get; set; }
    }
}
