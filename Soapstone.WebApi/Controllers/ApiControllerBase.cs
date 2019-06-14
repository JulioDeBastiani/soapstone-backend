using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Soapstone.WebApi.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        public async Task<ActionResult<T>> ExecuteAsync<T>(Func<Task<ActionResult<T>>> functionAsync)
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

        public async Task<ActionResult> ExecuteAsync(Func<Task<ActionResult>> functionAsync)
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
    }
}