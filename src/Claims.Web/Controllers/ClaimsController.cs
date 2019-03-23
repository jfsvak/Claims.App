using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Claims.Business.Models;
using System.Net;
using System.Web.Http;
using Claims.Business.Service;

namespace Claims.Web.Controllers
{
    [Route("api/claims")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        /// <summary>
        /// Takes an email text and creates a claim accordingly
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("email")]
        [ProducesResponseType(200, Type = typeof(Claim))]
        public ActionResult<Claim> Email([FromBody] string email)
        {
            Debug.WriteLine($"Email submitted[{email}]");

            var claim = new ClaimService().ParseClaim(email);
            Debug.WriteLine($"Claim created with id[{claim.Id}]");

            return Ok(claim);
        }

        /// <summary>
        /// Gets all the claims
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<Claim>> Claims()
        {
            return new List<Claim> {
                new Claim
                    {
                        Id = Guid.NewGuid(),
                        Event = new Event { Vendor = "My Steakhouse", Description = "Team building", Date = DateTime.Now },
                        Expense = new Expense { Total = 199.9m, PaymentMethod = "Personal card" }
                    },
                new Claim
                    {
                        Id = Guid.NewGuid(),
                        Event = new Event { Vendor = "The Bowling Alley", Description = "Team building", Date = DateTime.Now },
                        Expense = new Expense { Total = 456.65m, PaymentMethod = "Company card" }
                    }
            };
        }
    }
}