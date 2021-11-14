using Business;
using Business.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HiddenVilla_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiddenVilla_ApiPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmenityController : ControllerBase
    {
        private readonly IAmenityRepository _amenityRepository;

        public AmenityController(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAmenities()
        {
            var amenities = await _amenityRepository.GetAllAmenities();
            return Ok(amenities);
        }

        [HttpGet("{amenityId}")]
        public async Task<IActionResult> GetAmenity(int? amenityId)
        {
            if (amenityId == null)
            {
                return BadRequest(new ErrorModel()
                {
                    Title = "",
                    ErrorMessage = "Invalid Amenity ID",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var amenityDetails = await _amenityRepository.GetAmenity(amenityId.Value);
            if (amenityDetails == null)
            {
                return BadRequest(new ErrorModel()
                {
                    Title = "",
                    ErrorMessage = "Invalid Amenity ID",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            return Ok(amenityDetails);
        }
    }
}
