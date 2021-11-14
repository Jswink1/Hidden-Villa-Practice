using Microsoft.AspNetCore.Components;
using HiddenVilla_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiddenVilla_ServerPractice.Services;
using Business.Repository.IRepository;
using Microsoft.JSInterop;

namespace HiddenVilla_ServerPractice.Pages.Amenities
{
    public partial class AmenitiesUpsert : ComponentBase
    {
        [Inject]
        public IAmenityRepository amenityRepository { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }
        [Inject]
        public IJSRuntime jsRuntime { get; set; }

        [Parameter]
        public int? Id { get; set; }

        private AmenityDTO AmenityModel { get; set; } = new();
        private string Title { get; set; } = "Create";
        internal bool IsDeleteProcessing { get; set; } = false;

        protected async override Task OnInitializedAsync()
        {
            // If we are updating
            if (Id != null)
            {
                Title = "Update";

                // Populate input fields
                AmenityModel = await amenityRepository.GetAmenity(Id.Value);
            }
            // If we are creating
            else
            {
                // Initialize
                AmenityModel = new AmenityDTO();
            }
        }

        private async Task HandleAmenityUpsert()
        {
            try
            {
                IsDeleteProcessing = true;

                var amenityDetailsByName = await amenityRepository.IsAmenityUnique(AmenityModel.Name, AmenityModel.Id);
                if (amenityDetailsByName != null)
                {
                    await jsRuntime.ToastrError("Amenity Name already exists.");
                    return;
                }

                // If an Amenity is being updated
                if (AmenityModel.Id != 0 && Title == "Update")
                {
                    var updateAmenityResult = await amenityRepository.UpdateAmenity(AmenityModel.Id, AmenityModel);
                    await jsRuntime.ToastrSuccess("Amenity updated successfully");
                }
                // If an Amenity is being created
                else
                {
                    var createAmenityResult = await amenityRepository.CreateAmenity(AmenityModel);
                    AmenityModel = new AmenityDTO();
                    await jsRuntime.ToastrSuccess("Amenity created successfully");
                }

                IsDeleteProcessing = false;
                navigationManager.NavigateTo("amenities");
            }
            catch (Exception)
            {
                IsDeleteProcessing = false;
            }
        }
    }
}
