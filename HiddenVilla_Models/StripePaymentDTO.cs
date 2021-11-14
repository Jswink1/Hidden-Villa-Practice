using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenVilla_Models
{
    public class StripePaymentDTO
    {
        public string ProductName { get; set; }
        public long Amount { get; set; }

        // Image of the room they are buying
        public string ImageUrl { get; set; }

        // URL that the user will return to after making the payment
        public string ReturnUrl { get; set; }
    }
}
