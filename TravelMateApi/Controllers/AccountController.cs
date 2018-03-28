using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using TravelMateApi.Database;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TravelMateApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        [HttpPut("refreshtoken")]
        public void Refresh([FromQuery] string uid, [FromQuery] string token)
        {
            var databaseFactory = new DatabaseFactory();
            databaseFactory.RefreshToken(uid, token);
        }
    }
}
