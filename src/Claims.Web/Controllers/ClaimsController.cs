using Claims.Business.Models;
using Claims.Business.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Claims.Web.Controllers
{
    [Route("api/claims")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger logger;

        public ClaimsController(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<ClaimsController>();
        }

        /// <summary>
        /// Takes an email text and creates a Claim
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("email")]
        [ProducesResponseType(200, Type = typeof(Claim))]
        [ProducesResponseType(400)]
        public IActionResult Email([FromBody] string email)
        {
            logger.LogDebug($"Email submitted [{email}]");

            try {
                var claim = new ClaimService().ParseClaim(email);

                logger.LogDebug($"Claim created with id[{claim.Id}]");

                return Ok(claim);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
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