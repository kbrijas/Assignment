using Microsoft.AspNetCore.Mvc;
using WebApplication1.Middlewares.Extension;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// This is a shared controller which needs to be implemented by all other controller base
    /// </summary>
    [ApiController]
    [MiddlewareFilter(typeof(RateLimterMiddlewarePipeline))]
    public class BaseController : ControllerBase
    {
      
    }
}
