using AutoMapper;
using Business.Repository.IRepository;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using HiddenVilla_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public AmenityRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<AmenityDTO> CreateAmenity(AmenityDTO amenityDTO)
        {
            Amenity amenity = _mapper.Map<AmenityDTO, Amenity>(amenityDTO);
            amenity.CreatedDate = DateTime.Now;
            amenity.CreatedBy = "";

            var addedAmenity = await _db.Amenities.AddAsync(amenity);
            await _db.SaveChangesAsync();

            return _mapper.Map<Amenity, AmenityDTO>(addedAmenity.Entity);
        }

        public async Task<int> DeleteAmenity(int amenityId)
        {
            var amenityDetails = await _db.Amenities.FindAsync(amenityId);

            if (amenityDetails != null)
            {
                _db.Amenities.Remove(amenityDetails);
                return await _db.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<IEnumerable<AmenityDTO>> GetAllAmenities()
        {
            try
            {
                IEnumerable<AmenityDTO> hotelAmenityDTOs =
                    _mapper.Map<IEnumerable<Amenity>, IEnumerable<AmenityDTO>>(await _db.Amenities.ToListAsync());

                return hotelAmenityDTOs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<AmenityDTO> GetAmenity(int amenityId)
        {
            try
            {
                AmenityDTO amenity =
                    _mapper.Map<Amenity, AmenityDTO>(await _db.Amenities.FirstOrDefaultAsync(x => x.Id == amenityId));

                return amenity;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<AmenityDTO> IsAmenityUnique(string name, int amenityId = 0)
        {
            try
            {
                if (amenityId == 0)
                {
                    AmenityDTO amenity = _mapper.Map<Amenity, AmenityDTO>(
                                                     await _db.Amenities.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower()));

                    return amenity;
                }
                else
                {
                    AmenityDTO amenity = _mapper.Map<Amenity, AmenityDTO>(
                                                     await _db.Amenities.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower()
                                                     && x.Id != amenityId));

                    return amenity;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<AmenityDTO> UpdateAmenity(int amenityId, AmenityDTO amenityDTO)
        {
            try
            {
                if (amenityId == amenityDTO.Id)
                {
                    Amenity amenityDetails = await _db.Amenities.FindAsync(amenityId);
                    Amenity amenity = _mapper.Map<AmenityDTO, Amenity>(amenityDTO, amenityDetails);
                    amenity.UpdatedBy = "";
                    amenity.UpdatedDate = DateTime.Now;

                    var updatedAmenity = _db.Amenities.Update(amenity);
                    await _db.SaveChangesAsync();

                    return _mapper.Map<Amenity, AmenityDTO>(updatedAmenity.Entity);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
    }
}
