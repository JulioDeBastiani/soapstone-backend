using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Soapstone.Domain;
using Soapstone.Domain.Interfaces;
using Soapstone.WebApi.InputModels;
using Soapstone.WebApi.ViewModels;

namespace Soapstone.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // TODO security
    public class UsersController : ApiControllerBase
    {
        private IRepository<User> _usersRepository;

        public UsersController(IRepository<User> usersRepository)
        {
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Gets a list of users
        /// </summary>
        /// <param name="inputModel">Page information</param>
        /// <returns></returns>
        [HttpGet]
        public Task<ActionResult<IEnumerable<UserViewModel>>> GetUsersAsync([FromQuery] PageInputModel inputModel = null)
            => ExecuteAsync<IEnumerable<UserViewModel>>(async () =>
            {
                var skip = inputModel?.Skip ?? 0; // TODO defaults
                var take = inputModel?.Take ?? 25; // TODO defaults

                var users = await _usersRepository.GetPageAsync(skip, take);

                if (!users.Any())
                    return NoContent();

                return Ok(users.Select(u => (UserViewModel) u));
            });

        /// <summary>
        /// Gets a specific user
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public Task<ActionResult<UserViewModel>> GetUserAsync(Guid id)
            => ExecuteAsync<UserViewModel>(async () =>
            {
                var user = await _usersRepository.GetByIdAsync(id);
                
                if (user == null)
                    return NotFound();

                return Ok((UserViewModel) user);
            });

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult<UserViewModel>> PostAsync([FromBody] UserInputModel inputModel)
            => ExecuteAsync<UserViewModel>(async () =>
            {
                return Ok(await _usersRepository.AddAsync(inputModel));
            });

        /// <summary>
        /// Changes an user's password
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <param name="inputModel">New password</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public Task<ActionResult> PutAsync(Guid id, [FromBody] ChangePasswordInputModel inputModel)
            => ExecuteAsync(async () =>
            {
                if (inputModel == null)
                    throw new ArgumentNullException(nameof(inputModel));

                var user = await _usersRepository.GetByIdAsync(id);

                if (user == null)
                    return NotFound();

                user.ChangePassword(inputModel.OldPassword, inputModel.NewPassword);
                return Ok();
            });

        /// <summary>
        /// Deletes an user
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        // TODO change to logical deletion
        public Task<ActionResult> DeleteAsync(Guid id)
            => ExecuteAsync(async () =>
            {
                var user = await _usersRepository.GetByIdAsync(id);

                if (user == null)
                    return NotFound();

                await _usersRepository.DeleteAsync(user);
                return Ok();
            });
    }
}