using System;
using Soapstone.Domain;

namespace Soapstone.WebApi.ViewModels
{
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set;}
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Upvoted { get; set; }
        public bool Downvoted { get; set; }
        public bool Saved { get; set; }
        public bool Reported { get; set; }

        public static implicit operator PostViewModel(Post post)
        {
            return new PostViewModel
            {
                Id = post.Id,
                UserId = post.UserId,
                Author = post.User?.Username,
                Message = post.Message,
                ImageUrl = post.ImageUrl,
                Latitude = post.Latitude,
                Longitude = post.Longitude,
                CreatedAt = post.CreatedAt,
                Upvoted = false,
                Downvoted = false,
                Saved = false,
                Reported = false
            };
        }
    }
}