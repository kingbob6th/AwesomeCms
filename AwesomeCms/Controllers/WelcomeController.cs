using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AwesomeCms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WelcomeController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "This is the thing you are looking for";
        }
    }
}
