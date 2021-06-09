using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    // We can use any authorization scheme here! I prefer to set a default scheme in the Startup.cs
   // [Authorize(/*AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme*/)]
    public class PostsController : Controller
    {
        [HttpGet]
        [Authorize]
        [Route("/posts")]
        public IActionResult Posts()
        {
            return Ok(new {msg = "you got it!!!"});
        }
    }
}