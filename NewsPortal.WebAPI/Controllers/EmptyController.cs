using Microsoft.AspNetCore.Mvc;

namespace NewsPortal.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmptyController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            return "hi";
        }
    }
}