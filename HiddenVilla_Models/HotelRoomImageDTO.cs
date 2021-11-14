using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenVilla_Models
{
    public class HotelRoomImageDTO
    {
        public int Id { get; set; }
        public string RoomImageUrl { get; set; }

        public int RoomId { get; set; }
    }
}
