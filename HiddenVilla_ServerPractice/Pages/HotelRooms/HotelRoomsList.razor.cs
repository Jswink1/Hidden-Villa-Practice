using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using HiddenVilla_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiddenVilla_ServerPractice.Services;
using Business.Repository.IRepository;
using HiddenVilla_ServerPractice.Services.IServices;

namespace HiddenVilla_ServerPractice.Pages.HotelRooms
{
    public partial class HotelRoomsList : ComponentBase
    {
        [Inject]
        public IHotelRoomRepository hotelRoomRepository { get; set; }
        [Inject]
        public IJSRuntime jsRuntime { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }
        [Inject]
        public IFileUpload fileUpload { get; set; }

        private IEnumerable<HotelRoomDTO> HotelRooms { get; set; } = new List<HotelRoomDTO>();
        private int? DeleteRoomId { get; set; } = null;
        private bool IsDeleteProcessing { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            HotelRooms = await hotelRoomRepository.GetAllHotelRooms();
        }

        private async Task HandleDelete(int roomId)
        {
            // Determine which room the user wants to delete
            DeleteRoomId = roomId;
            // Show Javascript Delete Confirmation Pop-up
            await jsRuntime.InvokeVoidAsync("ShowDeleteConfirmationModal");
        }

        public async Task ProcessDelete_Click(bool isConfirmed)
        {
            IsDeleteProcessing = true;
            if (isConfirmed && DeleteRoomId != null)
            {
                // Load the HotelRoom and delete the image files from the webroot
                HotelRoomDTO hotelRoom = await hotelRoomRepository.GetHotelRoom(DeleteRoomId.Value);
                foreach (var image in hotelRoom.HotelRoomImages)
                {
                    // Get the exact name of the image from the URL and delete
                    var imageName = image.RoomImageUrl.Replace($"{navigationManager.BaseUri}RoomImages/", "");
                    fileUpload.DeleteFile(imageName);
                }

                // Delete Hotel Room
                await hotelRoomRepository.DeleteHotelRoom(DeleteRoomId.Value);
                await jsRuntime.ToastrSuccess("Hotel Room Deleted Successfully");

                // Repopulate List of Hotel Rooms
                HotelRooms = await hotelRoomRepository.GetAllHotelRooms();
            }

            // Hide Javascript Delete Confirmation Pop-up
            await jsRuntime.InvokeVoidAsync("HideDeleteConfirmationModal");
            IsDeleteProcessing = false;
        }
    }
}
