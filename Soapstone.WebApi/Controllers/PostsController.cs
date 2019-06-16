using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Soapstone.Domain;
using Soapstone.Domain.Defaults;
using Soapstone.Domain.Interfaces;
using Soapstone.WebApi.InputModels;
using Soapstone.WebApi.Services;
using Soapstone.WebApi.ViewModels;

namespace Soapstone.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // TODO security
    public class PostsController : ApiControllerBase
    {
        private IRepository<Post> _postsRepository;
        private PostsService _postsService;

        public PostsController(IRepository<Post> postsRepository, PostsService postsService)
        {
            _postsRepository = postsRepository;
            _postsService = postsService;
        }

        /// <summary>
        /// Gets the posts on a radius of 50 meters from the user
        /// Posts ordered by rating
        /// </summary>
        /// <param name="inputModel">Page and geolocation information</param>
        /// <returns></returns>
        // TODO get user from claims
        [HttpGet]
        public Task<ActionResult<IEnumerable<PostViewModel>>> GetPostsAsync([FromQuery] PostsPageInputModel inputModel)
            => ExecuteAsync<IEnumerable<PostViewModel>>(async () =>
            {
                var posts = await _postsService.GetNearbyPostsAsync(inputModel);

                if (!posts.Any())
                    return NoContent();

                return Ok(posts);
            });

        /// <summary>
        /// Gets the top posts
        /// </summary>
        /// <param name="inputModel">Page information</param>
        /// <returns></returns>
        [HttpGet("top")]
        // TODO timeframe
        public Task<ActionResult<IEnumerable<PostViewModel>>> GetPostsAsync([FromQuery] PageInputModel inputModel = null)
            => ExecuteAsync<IEnumerable<PostViewModel>>(async () =>
            {
                var skip = inputModel?.Skip ?? PaginationDefaults.DefaultSkip;
                var take = inputModel?.Take ?? PaginationDefaults.DefaultTake;

                var posts = await _postsRepository.GetPageAsync(p => p.Rating, skip, take);

                if (!posts.Any())
                    return NoContent();

                return Ok(posts.Select(p => (PostViewModel) p));
            });

        /// <summary>
        /// Creates a New Post
        /// </summary>
        /// <param name="inputModel">Post information</param>
        /// <returns></returns>
        // TODO change reponse to created
        [HttpPost]
        public Task<ActionResult<PostViewModel>> PostAsync([FromBody] PostInputModel inputModel)
            => ExecuteAsync<PostViewModel>(async () =>
            {
                var post = (Post) inputModel;

                if (await _postsRepository.AddAsync(post) != 1)
                    return BadRequest();

                return Ok((PostViewModel) post);
            });

        /// <summary>
        /// Deletes a post
        /// </summary>
        /// <param name="id">Id of the post</param>
        /// <returns></returns>
        // TODO logical deletion
        [HttpDelete("{id}")]
        public Task<ActionResult> DeleteAsync(Guid id)
            => ExecuteAsync(async () =>
            {
                var post = await _postsRepository.GetByIdAsync(id);

                if (post == null)
                    return NotFound();

                await _postsRepository.DeleteAsync(post);
                return Ok();
            });

        /// <summary>
        /// Upvotes a post
        /// </summary>
        /// <param name="id">Id of the post</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        [HttpPut("{id}/upvote/{userId}")]
        // TODO get user from claims
        public Task<ActionResult> UpvoteAsync(Guid id, Guid userId)
            => ExecuteAsync(async () =>
            {
                await _postsService.UpvoteAsync(id, userId);
                return Ok();
            });

        /// <summary>
        /// Downvotes a post
        /// </summary>
        /// <param name="id">Id of the post</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        [HttpPut("{id}/downvote/{userId}")]
        // TODO get user from claims
        public Task<ActionResult> DownvoteAsync(Guid id, Guid userId)
            => ExecuteAsync(async () =>
            {
                await _postsService.DownvoteAsync(id, userId);
                return Ok();
            });

        /// <summary>
        /// Saves a post
        /// </summary>
        /// <param name="id">Id of the post</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        [HttpPut("{id}/save/{userId}")]
        // TODO get user from claims
        public Task<ActionResult> SaveAsync(Guid id, Guid userId)
            => ExecuteAsync(async () =>
            {
                await _postsService.SaveAsync(id, userId);
                return Ok();
            });

        /// <summary>
        /// Report a post
        /// </summary>
        /// <param name="id">Id of the post</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        [HttpPut("{id}/report/{userId}")]
        // TODO get user from claims
        public Task<ActionResult> ReportAsync(Guid id, Guid userId)
            => ExecuteAsync(async () =>
            {
                await _postsService.ReportAsync(id, userId);
                return Ok();
            });
    }
}