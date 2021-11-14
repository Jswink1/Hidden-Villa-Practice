using HiddenVilla_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IAmenityRepository
    {
        public Task<AmenityDTO> CreateAmenity(AmenityDTO amenityDTO);
        public Task<AmenityDTO> UpdateAmenity(int amenityId, AmenityDTO amenityDTO);
        public Task<AmenityDTO> GetAmenity(int amenityId);
        public Task<int> DeleteAmenity(int amenityId);
        public Task<IEnumerable<AmenityDTO>> GetAllAmenities();
        public Task<AmenityDTO> IsAmenityUnique(string name, int amenityId = 0);
    }
}
