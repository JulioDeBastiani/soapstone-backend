using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soapstone.Domain;
using Soapstone.Domain.Defaults;
using Soapstone.Domain.Interfaces;
using Soapstone.WebApi.InputModels;
using Soapstone.WebApi.ViewModels;

namespace Soapstone.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ApiControllerBase
    {
        private IRepository<User> _usersRepository;
        private IRepository<Post> _postsRepository;
        private IRepository<SavedPost> _savedPostsRepository;

        public UsersController(
            IRepository<User> usersRepository,
            IRepository<Post> postsRepository,
            IRepository<SavedPost> savedPostsRepository)
        {
            _usersRepository = usersRepository;
            _postsRepository = postsRepository;
            _savedPostsRepository = savedPostsRepository;
        }

        /// <summary>
        /// Gets a list of users
        /// </summary>
        /// <param name="inputModel">Page information</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public Task<ActionResult<IEnumerable<UserViewModel>>> GetUsersAsync([FromQuery] PageInputModel inputModel = null)
            => ExecuteAsync<IEnumerable<UserViewModel>>(async () =>
            {
                var skip = inputModel?.Skip ?? PaginationDefaults.DefaultSkip;
                var take = inputModel?.Take ?? PaginationDefaults.DefaultTake;

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
        [Authorize]
        public Task<ActionResult<UserViewModel>> GetUserAsync(Guid id)
            => ExecuteAsync<UserViewModel>(async () =>
            {
                var user = await _usersRepository.GetByIdAsync(id);
                
                if (user == null)
                    return NotFound();

                return Ok((UserViewModel) user);
            });

        /// <summary>
        /// Gets an user's posts
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <param name="inputModel">Page information</param>
        /// <returns></returns>
        [HttpGet("{id}/posts")]
        [Authorize]
        public Task<ActionResult<IEnumerable<PostViewModel>>> GetPostsAsync(Guid id, [FromQuery] PageInputModel inputModel = null)
            => ExecuteAsync<IEnumerable<PostViewModel>>(async () =>
            {
                var skip = inputModel?.Skip ?? PaginationDefaults.DefaultSkip;
                var take = inputModel?.Take ?? PaginationDefaults.DefaultTake;

                var posts = await _postsRepository.GetPageDescendingAsync(p => p.UserId == id, p => p.CreatedAt, skip, take);

                if (!posts.Any())
                    return NoContent();

                return Ok(posts.Select(p => (PostViewModel) p));
            });

        /// <summary>
        /// Gets an user's saved posts
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <param name="inputModel">Page information</param>
        /// <returns></returns>
        [HttpGet("{id}/saved")]
        [Authorize]
        public Task<ActionResult<IEnumerable<PostViewModel>>> GetSavedAsync(Guid id, [FromQuery] PageInputModel inputModel = null)
            => ExecuteAsync<IEnumerable<PostViewModel>>(async () =>
            {
                var skip = inputModel?.Skip ?? PaginationDefaults.DefaultSkip;
                var take = inputModel?.Take ?? PaginationDefaults.DefaultTake;

                var posts = await _savedPostsRepository.GetPageDescendingAsync(p => p.UserId == id, p => p.CreatedAt, p => p.Include(e => e.Post).ThenInclude(e => e.User), skip, take);

                if (!posts.Any())
                    return NoContent();

                return Ok(posts.Select(p => (PostViewModel) p.Post));
            });

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="inputModel">User information</param>
        /// <returns></returns>
        // TODO change rerponse to created
        [HttpPost]
        public Task<ActionResult<UserViewModel>> PostAsync([FromBody] UserInputModel inputModel)
            => ExecuteAsync<UserViewModel>(async () =>
            {
                if (await _usersRepository.AnyAsync(u => u.Username == inputModel.Username.Trim()))
                    return BadRequest("Username already taken");

                var user = (User) inputModel;

                if (await _usersRepository.AddAsync(user) != 1)
                    return BadRequest();

                return Ok((UserViewModel) user);
            });

        /// <summary>
        /// Changes an user's password
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <param name="inputModel">New password</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
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
        [Authorize]
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