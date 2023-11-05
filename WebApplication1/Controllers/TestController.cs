using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{

    [Route("api/[controller]")]
    public class TestController : BaseController
    {
        public TestController() { }

        [HttpGet]
        public IActionResult Get() 
        {
            return Ok("ApiWorks");
        }
    }
}
