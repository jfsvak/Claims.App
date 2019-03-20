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

namespace Claims.Web.Controllers
{
    [Route("api/claims")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
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
                        Id = "1",
                        Event = new Event { Vendor = "My Steakhouse", Description = "Team building", Date = DateTime.Now },
                        Expense = new Expense { Total = 199.9, PaymentMethod = "Personal card" }
                    },
                new Claim
                    {
                        Id = "2",
                        Event = new Event { Vendor = "The Bowling Alley", Description = "Team building", Date = DateTime.Now },
                        Expense = new Expense { Total = 456.65, PaymentMethod = "Company card" }
                    }
            };
        }

        /// <summary>
        /// Takes an email text and creates a claim accordingly
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("email")]
        [ProducesResponseType(200, Type = typeof(Claim))]
        public ActionResult<Claim> Email([FromBody] string email)
        {
            Debug.WriteLine("Email submitted[{0}]", email);
            
            var dummyClaim = new Claim
            {
                Id = "1",
                Event = new Event { Vendor = "My Steakhouse", Description = "Team building", Date = DateTime.Now },
                Expense = new Expense { Total = 199.9, PaymentMethod = "Personal card" }
            };

            Debug.WriteLine("Claim created with id[{0}]", dummyClaim.Id);
            // return the claims id or the entire structure for the json for the created claim
            return Ok(dummyClaim); // dummyClaim);
        }
    }
}