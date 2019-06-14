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
    public class PostsController : ApiControllerBase
    {
        private IRepository<Post> _postsRepository;

        public PostsController(IRepository<Post> postsRepository)
        {
            _postsRepository = postsRepository;
        }

        // FIXME not using any particular order, change to date
        public Task<ActionResult<IEnumerable<PostViewModel>>> GetPostsAsync([FromBody] PostsPageInputModel inputModel = null)
            => ExecuteAsync<IEnumerable<PostViewModel>>(async () =>
            {
                var skip = inputModel?.Skip ?? 0; // TODO defaults
                var take = inputModel?.Take ?? 25; // TODO defaults
                var predicate = default(Func<Post, bool>);

                if (inputModel?.Latitude != null
                    && inputModel?.Longitude != null)
                    predicate = p => true;

                // TODO page by date
                var posts = await _postsRepository.GetPageAsync(predicate, skip, take);

                if (posts.Any())
                    return NoContent();

                return Ok(posts.Select(p => (PostViewModel) p));
            });
    }
}