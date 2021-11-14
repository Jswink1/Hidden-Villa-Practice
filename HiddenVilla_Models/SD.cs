using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenVilla_Models
{
    public static class SD
    {
        // User Roles
        public const string Role_Admin = "Admin";
        public const string Role_Customer = "Customer";
        public const string Role_Employee = "Employee";

        // Local Storage Key Strings
        public const string Local_RoomSearchInput = "RoomSearchInput";
        public const string Local_RoomOrderDetails = "RoomOrderDetails";
        public const string Local_Token = "JWT Token";
        public const string Local_UserDetails = "User Details";

        // Order Status Codes
        public const string Status_Pending = "Pending";
        public const string Status_Booked = "Booked";
        public const string Status_CheckedIn = "CheckedIn";
        public const string Status_CheckedOut_Completed = "CheckedOut";
        public const string Status_NoShow = "NoShow";
        public const string Status_Cancelled = "Cancelled";
    }
}
