using HiddenVilla_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiddenVilla_ClientPractice.Services.IService
{
    public interface IAmenityService
    {
        public Task<IEnumerable<AmenityDTO>> GetAmenities();
    }
}
