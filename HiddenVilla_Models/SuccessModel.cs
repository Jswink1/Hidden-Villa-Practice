using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenVilla_Models
{
    public class SuccessModel
    {
        public string Title { get; set; }
        public int StatusCode { get; set; }
        public string SuccessMessage { get; set; }

        // Object for any random/misc data that needs to be added to the model
        public object Data { get; set; }
    }
}
