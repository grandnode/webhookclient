using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandWebhookExample.Models
{
    public class GrandWebhook
    {
        public int Type { get; set; }
        public string Body { get; set; }
        public DateTime CreationTimeUtc { get; set; }
    }
}
