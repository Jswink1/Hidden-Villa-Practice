using HiddenVilla_ClientPractice.Services.IService;
using HiddenVilla_Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HiddenVilla_ClientPractice.Services
{
    public class AmenityService : IAmenityService
    {
        private readonly HttpClient _client;

        public AmenityService(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<AmenityDTO>> GetAmenities()
        {
            var response = await _client.GetAsync($"api/amenity");
            var content = await response.Content.ReadAsStringAsync();
            var amenities = JsonConvert.DeserializeObject<IEnumerable<AmenityDTO>>(content);
            return amenities;
        }
    }
}
