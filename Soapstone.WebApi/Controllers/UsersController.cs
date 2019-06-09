using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Soapstone.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Test()
            => await Task.Run(() => Ok());
    }
}