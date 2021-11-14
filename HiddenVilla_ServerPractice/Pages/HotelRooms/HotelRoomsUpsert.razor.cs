using Blazored.TextEditor;
using Business;
using Business.Repository;
using Business.Repository.IRepository;
using HiddenVilla_ServerPractice.Services;
using HiddenVilla_ServerPractice.Services.IServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using HiddenVilla_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiddenVilla_ServerPractice.Pages.HotelRooms
{
    public partial class HotelRoomsUpsert : ComponentBase
    {
        [Inject]
        public IHotelRoomRepository hotelRoomRepository { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }
        [Inject]
        public IJSRuntime jsRuntime { get; set; }
        [Inject]
        public IHotelImagesRepository hotelImagesRepository { get; set; }
        [Inject]
        public IFileUpload fileUpload { get; set; }

        [Parameter]
        public int? Id { get; set; }
        private HotelRoomDTO HotelRoomModel { get; set; } = new HotelRoomDTO();
        private string Title { get; set; } = "Create";
        private HotelRoomImageDTO RoomImage { get; set; } = new HotelRoomImageDTO();

        // Blazored Editor Component
        public BlazoredTextEditor QuillHtml { get; set; } = new();

        // Image Loading Trigger
        private bool IsImageLoading { get; set; } = false;

        // List of Images the user intends to delete
        private List<string> DeletedImages { get; set; } = new();

        // Authentication state to check if user is Admin
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //If user is not Admin, redirect them to login page
            var authenticationState = await AuthenticationState;
            if (authenticationState.User.IsInRole(SD.Role_Admin) == false)
            {
                // ReturnURL for the user to return to this page after they login
                var uri = new Uri(navigationManager.Uri);

                navigationManager.NavigateTo($"/identity/account/login?returnUrl={uri.LocalPath}");
            }

            // If we are updating
            if (Id != null)
            {
                Title = "Update";

                // Populate input fields and images
                HotelRoomModel = await hotelRoomRepository.GetHotelRoom(Id.Value);
                if (HotelRoomModel?.HotelRoomImages != null)
                {
                    HotelRoomModel.ImageUrls = HotelRoomModel.HotelRoomImages.Select(u => u.RoomImageUrl).ToList();
                }
            }
            // If we are creating
            else
            {
                // Initialize
                HotelRoomModel = new HotelRoomDTO();
            }
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                // Quill does not need to be loaded if this isnt the first render
                return;
            }

            // Populate Quill rich text area
            bool loading = true;
            while (loading)
            {
                try
                {
                    if (string.IsNullOrEmpty(HotelRoomModel.Details) == false)
                    {
                        await this.QuillHtml.LoadHTMLContent(HotelRoomModel.Details);
                    }
                    loading = false;
                }
                catch (Exception)
                {
                    await Task.Delay(10);
                    loading = true;
                }
            }
        }

        private async Task HandleHotelRoomUpsert()
        {
            try
            {
                // Check if room name is unique
                var roomDetailsByName = await hotelRoomRepository.IsRoomUnique(HotelRoomModel.Name, HotelRoomModel.Id);
                if (roomDetailsByName != null)
                {
                    await jsRuntime.ToastrError("Room Name already exists.");
                    return;
                }

                // If a Hotel Room is being updated
                if (HotelRoomModel.Id != 0 && Title == "Update")
                {
                    // Retrieve Details input from Quill rich text area
                    HotelRoomModel.Details = await QuillHtml.GetHTML();

                    var updateRoomResult = await hotelRoomRepository.UpdateHotelRoom(HotelRoomModel.Id, HotelRoomModel);

                    // If images need to be added or deleted
                    if ((HotelRoomModel.ImageUrls != null && HotelRoomModel.ImageUrls.Any())
                        || (DeletedImages != null && DeletedImages.Any()))
                    {
                        // Check if there are any room images to be deleted
                        if (DeletedImages != null && DeletedImages.Any())
                        {
                            foreach (var deletedImageUrl in DeletedImages)
                            {
                                // Get the exact name of the image from the URL
                                var imageName = deletedImageUrl.Replace($"{navigationManager.BaseUri}RoomImages/", "");
                                // Delete the image file
                                var result = fileUpload.DeleteFile(imageName);
                                // Delete the image from the DB
                                await hotelImagesRepository.DeleteHotelRoomImageByImageUrl(deletedImageUrl);
                            }
                        }
                        await AddHotelRoomImage(updateRoomResult);
                    }
                    await jsRuntime.ToastrSuccess("Hotel Room updated successfully");
                }
                // If a Hotel Room is being created
                else
                {
                    // Retrieve Details input from Quill rich text area
                    HotelRoomModel.Details = await QuillHtml.GetHTML();

                    var createdResult = await hotelRoomRepository.CreateHotelRoom(HotelRoomModel);
                    await AddHotelRoomImage(createdResult);
                    await jsRuntime.ToastrSuccess("Hotel Room created successfully");
                }
            }
            catch (Exception)
            {
                throw;
            }

            navigationManager.NavigateTo("hotel-rooms");
        }

        private async Task AddHotelRoomImage(HotelRoomDTO roomDetails)
        {
            foreach (var imageUrl in HotelRoomModel.ImageUrls)
            {
                // Only create an image if an image with this url does not already exist in order to avoid duplicates
                if (HotelRoomModel.HotelRoomImages == null
                    || HotelRoomModel.HotelRoomImages.Where(x => x.RoomImageUrl == imageUrl).Count() == 0)
                {
                    RoomImage = new HotelRoomImageDTO()
                    {
                        RoomId = roomDetails.Id,
                        RoomImageUrl = imageUrl
                    };
                    await hotelImagesRepository.CreateHotelRoomImage(RoomImage);
                }
            }
        }

        private async Task HandleImageUpload(InputFileChangeEventArgs e)
        {
            // Start Spinner
            IsImageLoading = true;

            try
            {
                var images = new List<string>();
                if (e.GetMultipleFiles().Count > 0)
                {
                    foreach (var file in e.GetMultipleFiles())
                    {
                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(file.Name);
                        if (fileInfo.Extension.ToLower() == ".jpg"
                            || fileInfo.Extension.ToLower() == ".png"
                            || fileInfo.Extension.ToLower() == ".jpeg")
                        {
                            var uploadedImagePath = await fileUpload.UploadFile(file);
                            images.Add(uploadedImagePath);
                        }
                        else
                        {
                            await jsRuntime.ToastrError("Invalid Image File Extension");
                            return;
                        }
                    }

                    if (images.Any())
                    {
                        // If the Hotel Room already has image files uploaded
                        if (HotelRoomModel.ImageUrls != null && HotelRoomModel.ImageUrls.Any())
                        {
                            // Add the new uploads to the list
                            HotelRoomModel.ImageUrls.AddRange(images);
                        }
                        // If the Hotel Room does not already have image files uploaded
                        else
                        {
                            // Initialize the list, then add the new uploads
                            HotelRoomModel.ImageUrls = new List<string>();
                            HotelRoomModel.ImageUrls.AddRange(images);
                        }
                    }
                    else
                    {
                        await jsRuntime.ToastrError("Image uploading failed");
                        return;
                    }
                }

                // End Spinner
                IsImageLoading = false;
            }
            catch (Exception ex)
            {
                await jsRuntime.ToastrError(ex.Message);
            }
        }

        internal async Task DeletePhoto(string imageUrl)
        {
            try
            {
                // Find the index of the current image
                var imageIndex = HotelRoomModel.ImageUrls.FindIndex(x => x == imageUrl);
                // Get the exact name from the image URL
                var imageName = imageUrl.Replace($"{navigationManager.BaseUri}RoomImages/", "");

                // If a Hotel Room is being created
                if (HotelRoomModel.Id == 0 && Title == "Create")
                {
                    // Delete the image file
                    var result = fileUpload.DeleteFile(imageName);
                }
                // If a Hotel Room is being updated
                else
                {
                    // Initialize to prevent null exception
                    DeletedImages ??= new();
                    // Add the image to the delete list to be removed only when the user clicks "Update" button
                    DeletedImages.Add(imageUrl);
                }

                // Remove the image from the page UI
                HotelRoomModel.ImageUrls.RemoveAt(imageIndex);
            }
            catch (Exception ex)
            {
                await jsRuntime.ToastrError(ex.Message);
            }
        }
    }
}
