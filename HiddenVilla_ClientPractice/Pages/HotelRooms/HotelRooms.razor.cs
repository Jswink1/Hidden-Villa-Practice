using Blazored.LocalStorage;
using HiddenVilla_ClientPractice.Helper;
using HiddenVilla_ClientPractice.Models.ViewModel;
using HiddenVilla_ClientPractice.Services.IService;
using HiddenVilla_Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiddenVilla_ClientPractice.Pages.HotelRooms
{
    public partial class HotelRooms : ComponentBase
    {
        [Inject]
        public ILocalStorageService localStorage { get; set; }
        [Inject]
        public IJSRuntime jsRuntime { get; set; }
        [Inject]
        public IHotelRoomService hotelRoomService { get; set; }

        private HomeVM HomeModel { get; set; } = new();
        public IEnumerable<HotelRoomDTO> Rooms { get; set; } = new List<HotelRoomDTO>();
        private bool IsProcessing { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Placing a Task.Delay is required in this method in order for a debugging breakpoint to work properly
                await Task.Delay(1000);

                // Load the User Input Data from LocalStorage
                if (await localStorage.GetItemAsync<HomeVM>(SD.Local_RoomSearchInput) != null)
                {
                    HomeModel = await localStorage.GetItemAsync<HomeVM>(SD.Local_RoomSearchInput);
                }
                else
                {
                    HomeModel.NumberOfNights = 1;
                }

                // Load the Search Results
                await LoadRooms();
            }
            catch (Exception ex)
            {
                await jsRuntime.ToastrError(ex.Message);
            }
        }

        private async Task LoadRooms()
        {
        // Load the Room Results
        Rooms = await hotelRoomService.GetHotelRooms(HomeModel.StartDate.ToString("MM/dd/yyyy"),
                                                     HomeModel.EndDate.ToString("MM/dd/yyyy"));

        // Calculate the price of each room for the number of nights searched for
        foreach (var room in Rooms)
                    {
                        room.TotalAmount = room.RegularRate * HomeModel.NumberOfNights;
                        room.TotalDays = HomeModel.NumberOfNights;
                    }
        }

        private async Task SaveBookingInfo()
        {
            IsProcessing = true;

        // Recalculate Check - Out Date
        HomeModel.EndDate = HomeModel.StartDate.AddDays(HomeModel.NumberOfNights);

        // Apply new Check- Out Date to localstorage and reload the results
        await localStorage.SetItemAsync(SD.Local_RoomSearchInput, HomeModel);
            await LoadRooms();
            IsProcessing = false;
        }
    }
}
