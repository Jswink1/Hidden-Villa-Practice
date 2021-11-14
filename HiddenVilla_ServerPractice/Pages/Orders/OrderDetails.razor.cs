using Business.Repository.IRepository;
using HiddenVilla_Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiddenVilla_ServerPractice.Pages.Orders
{
    public partial class OrderDetails : ComponentBase
    {
        [Inject]
        public IRoomOrderDetailsRepository roomOrderDetailsRepository { get; set; }

        [Parameter]
        public int Id { get; set; }
        private bool IsLoading { get; set; } = false;
        private RoomOrderDetailsDTO HotelBooking { get; set; } = new() { HotelRoomDTO = new HotelRoomDTO() };

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;

            if (Id != 0)
            {
                HotelBooking = await roomOrderDetailsRepository.GetRoomOrderDetail(Id);
            }
            else
            {
                // error
            }

            IsLoading = false;
        }

        private async Task ConfirmCheckIn()
        {
            await roomOrderDetailsRepository.UpdateOrderStatus(HotelBooking.Id, SD.Status_CheckedIn);
            HotelBooking = await roomOrderDetailsRepository.GetRoomOrderDetail(Id);
        }

        private async Task ConfirmCheckOut()
        {
            await roomOrderDetailsRepository.UpdateOrderStatus(HotelBooking.Id, SD.Status_CheckedOut_Completed);
            HotelBooking = await roomOrderDetailsRepository.GetRoomOrderDetail(Id);
        }

        private async Task CancelBooking()
        {
            await roomOrderDetailsRepository.UpdateOrderStatus(HotelBooking.Id, SD.Status_Cancelled);
            HotelBooking = await roomOrderDetailsRepository.GetRoomOrderDetail(Id);
        }

        private async Task NoShowBooking()
        {
            await roomOrderDetailsRepository.UpdateOrderStatus(HotelBooking.Id, SD.Status_NoShow);
            HotelBooking = await roomOrderDetailsRepository.GetRoomOrderDetail(Id);
        }
    }
}
