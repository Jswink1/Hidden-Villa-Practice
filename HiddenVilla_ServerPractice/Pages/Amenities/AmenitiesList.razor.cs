using HiddenVilla_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiddenVilla_ServerPractice.Services;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Business.Repository.IRepository;

namespace HiddenVilla_ServerPractice.Pages.Amenities
{
    public partial class AmenitiesList : ComponentBase
    {
        [Inject]
        public IAmenityRepository amenityRepository { get; set; }
        [Inject]
        public IJSRuntime jsRuntime { get; set; }

        private IEnumerable<AmenityDTO> Amenities { get; set; } = new List<AmenityDTO>();
        private bool IsDeleteProcessing { get; set; } = false;
        // AmenityId which the user intends to delete
        private int? DeleteAmenityId { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            Amenities = await amenityRepository.GetAllAmenities();
        }

        private async Task HandleDelete(int amenityId)
        {
            // Determine which amenity the user wants to delete
            DeleteAmenityId = amenityId;
            // Show Javascript Delete Confirmation Pop-up
            await jsRuntime.InvokeVoidAsync("ShowDeleteConfirmationModal");
        }

        public async Task ProcessDelete_Click(bool isConfirmed)
        {
            IsDeleteProcessing = true;
            if (isConfirmed && DeleteAmenityId != null)
            {
                // Delete Amenity
                await amenityRepository.DeleteAmenity(DeleteAmenityId.Value);
                await jsRuntime.ToastrSuccess("Amenity Deleted Successfully");

                // Repopulate List of Hotel Rooms
                Amenities = await amenityRepository.GetAllAmenities();
            }

            // Hide Javascript Delete Confirmation Pop-up
            await jsRuntime.InvokeVoidAsync("HideDeleteConfirmationModal");
            IsDeleteProcessing = false;
        }
    }
}
