using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Soapstone.Domain;
using Soapstone.Domain.Defaults;
using Soapstone.Domain.Interfaces;
using Soapstone.WebApi.InputModels;
using Soapstone.WebApi.ViewModels;

namespace Soapstone.WebApi.Services
{
    public class PostService
    {
        private IRepository<Post> _postsRepository;

        public PostService(IRepository<Post> postsRepository)
        {
            _postsRepository = postsRepository;
        }

        public async Task<IEnumerable<PostViewModel>> GetNearbyPostsAsync(Guid userId, PostsPageInputModel inputModel)
        {
            if (inputModel == null)
                throw new ArgumentNullException(nameof(inputModel));

            var skip = inputModel.Skip ?? PaginationDefaults.DefaultSkip;
            var take = inputModel.Take ?? PaginationDefaults.DefaultTake;
            var latitude = inputModel.Latitude;
            var longitude = inputModel.Longitude;

            // TODO por algum motivo o get page nao ta funcionando direito
            // var posts = await _postsRepository
            //     .GetPageAsync(
            //         p =>
            //             p.Latitude < latitude + (latitude * 0.0005)
            //             && p.Latitude > latitude - (latitude * 0.0005)
            //             && p.Longitude < longitude + (longitude * 0.0005)
            //             && p.Longitude > longitude - (longitude * 0.0005),
            //         p => p.Rating,
            //         p => p
            //             .Include(e => e.User)
            //             .Include(e => e.Upvotes)
            //             .Include(e => e.Downvotes)
            //             .Include(e => e.SavedBy)
            //             .Include(e => e.Reports),
            //         skip,
            //         take);

            var posts = (await _postsRepository.GetQueryableAsync())
                .Where(p =>
                    p.Latitude < latitude + (latitude * ((latitude < 0) ? -0.0005 : 0.0005))
                    && p.Latitude > latitude - (latitude * ((latitude < 0) ? -0.0005 : 0.0005))
                    && p.Longitude < longitude + (longitude * ((longitude < 0) ? -0.0005 : 0.0005))
                    && p.Longitude > longitude - (longitude * ((longitude < 0) ? -0.0005 : 0.0005)))
                .OrderBy(p => p.Rating)
                .Include(e => e.User)
                .Include(e => e.Upvotes)
                .Include(e => e.Downvotes)
                .Include(e => e.SavedBy)
                .Include(e => e.Reports)
                .Skip(skip)
                .Take(take)
                .AsEnumerable();

            var viewModels = new List<PostViewModel>();

            foreach (var post in posts)
            {
                var viewModel = (PostViewModel) post;
                viewModel.Upvoted = post.Upvotes.Any(u => u.UserId == userId);
                viewModel.Downvoted = post.Downvotes.Any(d => d.UserId == userId);
                viewModel.Saved = post.SavedBy.Any(s => s.UserId == userId);
                viewModel.Reported = post.Upvotes.Any(r => r.UserId == userId);
                viewModels.Add(viewModel);
            }

            return viewModels;
        }

        public async Task UpvoteAsync(Guid postId, Guid userId)
        {
            var post = await _postsRepository.GetByIdAsync(postId, p => p
                .Include(e => e.Upvotes)
                .Include(e => e.Downvotes));

            var upvote = post.Upvotes.SingleOrDefault(u => u.UserId == userId);

            if (upvote == null)
            {
                upvote = new Upvote(userId, postId);
                post.Upvotes.Add(upvote);
            }
            else
                post.Upvotes.Remove(upvote);

            // TODO test to see if it is necessary to use an upvotes repository
            post.UpdateRating();
            await _postsRepository.UpdateAsync(post);
        }

        public async Task DownvoteAsync(Guid postId, Guid userId)
        {
            var post = await _postsRepository.GetByIdAsync(postId, p => p
                .Include(e => e.Upvotes)
                .Include(e => e.Downvotes));

            var downvote = post.Downvotes.SingleOrDefault(u => u.UserId == userId);

            if (downvote == null)
            {
                downvote = new Downvote(userId, postId);
                post.Downvotes.Add(downvote);
            }
            else
                post.Downvotes.Remove(downvote);

            // TODO test to see if it is necessary to use an downvotes repository
            post.UpdateRating();
            await _postsRepository.UpdateAsync(post);
        }

        public async Task SaveAsync(Guid postId, Guid userId)
        {
            var post = await _postsRepository.GetByIdAsync(postId, p => p
                .Include(e => e.SavedBy));

            var saved = post.SavedBy.SingleOrDefault(u => u.UserId == userId);

            if (saved == null)
            {
                saved = new SavedPost(userId, postId);
                post.SavedBy.Add(saved);
            }
            else
                post.SavedBy.Remove(saved);

            // TODO test to see if it is necessary to use an downvotes repository
            await _postsRepository.UpdateAsync(post);
        }

        public async Task ReportAsync(Guid postId, Guid userId)
        {
            var post = await _postsRepository.GetByIdAsync(postId, p => p
                .Include(e => e.Reports));

            var report = post.Reports.SingleOrDefault(u => u.UserId == userId);

            if (report == null)
            {
                report = new Report(userId, postId);
                post.Reports.Add(report);
            }
            else
                post.Reports.Remove(report);

            // TODO test to see if it is necessary to use an downvotes repository
            await _postsRepository.UpdateAsync(post);
        }
    }
}