using Blazored.LocalStorage;
using HiddenVilla_ClientPractice.Helper;
using HiddenVilla_ClientPractice.Models.ViewModel;
using HiddenVilla_ClientPractice.Services.IService;
using HiddenVilla_ClientPractice.ViewModels;
using HiddenVilla_Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiddenVilla_ClientPractice.Pages.HotelRooms
{
    public partial class RoomDetails : ComponentBase
    {
        [Inject]
        public IJSRuntime jsRuntime { get; set; }
        [Inject]
        public ILocalStorageService localStorage { get; set; }
        [Inject]
        public IHotelRoomService hotelRoomService { get; set; }
        [Inject]
        public IStripePaymentService stripePaymentService { get; set; }
        [Inject]
        public IRoomOrderDetailsService roomOrderDetailsService { get; set; }

        [Parameter]
        public int? Id { get; set; }

        public HotelRoomBookingVM HotelBooking { get; set; } = new();
        private int NumberOfNights { get; set; } = 1;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                HotelBooking.OrderDetails = new RoomOrderDetailsDTO();
                if (Id != null)
                {
                    if (await localStorage.GetItemAsync<HomeVM>(SD.Local_RoomSearchInput) != null)
                    {
                        // Load User Search Inputs from local storage
                        var roomInitialInfo = await localStorage.GetItemAsync<HomeVM>(SD.Local_RoomSearchInput);

                        // Load Hotel Room Search Result
                        HotelBooking.OrderDetails.HotelRoomDTO = await hotelRoomService.GetHotelRoomDetails(Id.Value,
                            roomInitialInfo.StartDate.ToString("MM/dd/yyyy"), roomInitialInfo.EndDate.ToString("MM/dd/yyyy"));

                        // Populate PageModel with HotelRoom details and User Search Inputs
                        NumberOfNights = roomInitialInfo.NumberOfNights;
                        HotelBooking.OrderDetails.CheckInDate = roomInitialInfo.StartDate;
                        HotelBooking.OrderDetails.CheckOutDate = roomInitialInfo.EndDate;
                        HotelBooking.OrderDetails.HotelRoomDTO.TotalDays = roomInitialInfo.NumberOfNights;
                        HotelBooking.OrderDetails.HotelRoomDTO.TotalAmount =
                        roomInitialInfo.NumberOfNights * HotelBooking.OrderDetails.HotelRoomDTO.RegularRate;
                    }
                    // If LocalStorage has null values then set PageModel Details to default values
                    else
                    {
                        HotelBooking.OrderDetails.HotelRoomDTO = await hotelRoomService.GetHotelRoomDetails(Id.Value,
                        DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.AddDays(1).ToString("MM/dd/yyyy"));
                        NumberOfNights = 1;
                        HotelBooking.OrderDetails.CheckInDate = DateTime.Now;
                        HotelBooking.OrderDetails.CheckOutDate = DateTime.Now.AddDays(1);
                        HotelBooking.OrderDetails.HotelRoomDTO.TotalDays = 1;
                        HotelBooking.OrderDetails.HotelRoomDTO.TotalAmount =
                        HotelBooking.OrderDetails.HotelRoomDTO.RegularRate;
                    }
                }

                // Populate input fields with user information
                if (await localStorage.GetItemAsync<UserDTO>(SD.Local_UserDetails) != null)
                {
                    var userInfo = await localStorage.GetItemAsync<UserDTO>(SD.Local_UserDetails);
                    HotelBooking.OrderDetails.UserId = userInfo.Id;
                    HotelBooking.OrderDetails.Name = userInfo.Name;
                    HotelBooking.OrderDetails.Email = userInfo.Email;
                    HotelBooking.OrderDetails.Phone = userInfo.PhoneNo;
                }
            }
            catch (Exception ex)
            {
                await jsRuntime.ToastrError(ex.Message);
            }
        }

        private async Task HandleNumberOfNightsChange(ChangeEventArgs e)
        {            
            NumberOfNights = Convert.ToInt32(e.Value.ToString());

            // Repopulate the HotelRoom after NumberOfNights change to see if hotel room is still available
            HotelBooking.OrderDetails.HotelRoomDTO =
                await hotelRoomService.GetHotelRoomDetails(Id.Value,
                                                           HotelBooking.OrderDetails.CheckInDate.ToString("MM/dd/yyyy"),
                                                           HotelBooking.OrderDetails.CheckInDate.AddDays(NumberOfNights).ToString("MM/dd/yyyy"));

            HotelBooking.OrderDetails.CheckOutDate = HotelBooking.OrderDetails.CheckInDate.AddDays(NumberOfNights);
            HotelBooking.OrderDetails.HotelRoomDTO.TotalDays = NumberOfNights;
            HotelBooking.OrderDetails.HotelRoomDTO.TotalAmount = NumberOfNights * HotelBooking.OrderDetails.HotelRoomDTO.RegularRate;
        }

        private async Task HandleCheckout()
        {
            if (!await HandleValidation())
            {
                return;
            }

            try
            {
                // Create Stripe Payment Details Object
                var paymentDTO = new StripePaymentDTO()
                {
                    Amount = Convert.ToInt32(HotelBooking.OrderDetails.HotelRoomDTO.TotalAmount * 100),
                    ProductName = HotelBooking.OrderDetails.HotelRoomDTO.Name,
                    ReturnUrl = "/hotel/room-details/" + Id
                };

                // Stripe Checkout
                var result = await stripePaymentService.CheckOut(paymentDTO);

                // Add StripeSessionId, and HotelRoomDTO details to OrderDetails
                HotelBooking.OrderDetails.StripeSessionId = result.Data.ToString();
                HotelBooking.OrderDetails.RoomId = HotelBooking.OrderDetails.HotelRoomDTO.Id;
                HotelBooking.OrderDetails.TotalCost = HotelBooking.OrderDetails.HotelRoomDTO.TotalAmount;

                // Save RoomOrderDetails to DB
                var roomOrderDetailsSaved = await roomOrderDetailsService.SaveRoomOrderDetails(HotelBooking.OrderDetails);

                // Save RoomOrderDetails to LocalStorage
                await localStorage.SetItemAsync(SD.Local_RoomOrderDetails, roomOrderDetailsSaved);

                // Redirect to the stripe session to see the stripe portal
                await jsRuntime.InvokeVoidAsync("redirectToCheckout", result.Data.ToString());
            }
            catch (Exception e)
            {
                await jsRuntime.ToastrError(e.Message);
            }
        }

        private async Task<bool> HandleValidation()
        {
            if (string.IsNullOrEmpty(HotelBooking.OrderDetails.Name))
            {
                await jsRuntime.ToastrError("Name cannot be empty");
                return false;
            }
            if (string.IsNullOrEmpty(HotelBooking.OrderDetails.Phone))
            {
                await jsRuntime.ToastrError("Phone cannot be empty");
                return false;
            }

            if (string.IsNullOrEmpty(HotelBooking.OrderDetails.Email))
            {
                await jsRuntime.ToastrError("Email cannot be empty");
                return false;
            }
            return true;

        }
    }
}
