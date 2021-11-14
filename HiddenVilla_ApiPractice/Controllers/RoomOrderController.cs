using Business.Repository.IRepository;
using HiddenVilla_Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiddenVilla_ApiPractice.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RoomOrderController : ControllerBase
    {
        private readonly IRoomOrderDetailsRepository _roomOrderDetailsRepository;

        public RoomOrderController(IRoomOrderDetailsRepository roomOrderDetailsRepository)
        {
            _roomOrderDetailsRepository = roomOrderDetailsRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoomOrderDetailsDTO details)
        {
            if (ModelState.IsValid)
            {
                var result = await _roomOrderDetailsRepository.Create(details);
                return Ok(result);
            }
            else
            {
                return BadRequest(new ErrorModel()
                {
                    ErrorMessage = "Error while creating Room Details/ Booking"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PaymentSuccessful([FromBody] RoomOrderDetailsDTO details)
        {
            // Load the stripe session
            var service = new SessionService();
            var sessionDetails = service.Get(details.StripeSessionId);

            // If payment status is successfull
            if (sessionDetails.PaymentStatus == "paid")
            {
                // Update the DB
                var result = await _roomOrderDetailsRepository.MarkPaymentSuccessful(details.Id);
                if (result == null)
                {
                    return BadRequest(new ErrorModel()
                    {
                        ErrorMessage = "Can not mark payment as successful"
                    });
                }

                return Ok(result);
            }
            else
            {
                return BadRequest(new ErrorModel()
                {
                    ErrorMessage = "Can not mark payment as successful"
                });
            }

        }
    }
}
