using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Soapstone.WebApi.Security;

namespace Soapstone.WebApi.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected async Task<ActionResult<T>> ExecuteAsync<T>(Func<Task<ActionResult<T>>> functionAsync)
        {
            try
            {
                return await functionAsync();
            }
            catch (ArgumentException)
            {
                // TODO log
                return BadRequest();
            }
            catch (Exception)
            {
                // TODO log
                // TODO change to 500
                return BadRequest();
            }
        }

        protected async Task<ActionResult> ExecuteAsync(Func<Task<ActionResult>> functionAsync)
        {
            try
            {
                return await functionAsync();
            }
            catch (ArgumentException)
            {
                // TODO log
                return BadRequest();
            }
            catch (Exception)
            {
                // TODO log
                // TODO change to 500
                return BadRequest();
            }
        }

        protected Guid GetUserId()
            => Guid.Parse(User.Claims.First(c => c.Type == DefaultClaims.UserId).Value);
    }
}