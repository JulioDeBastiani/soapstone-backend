using System;
using System.Collections.Generic;

namespace Soapstone.Domain
{
    public class Post : Entity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public string Message { get; private set; }
        public string ImageUrl { get; private set;}
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public bool Deleted { get; private set; }

        public ICollection<Upvote> Upvotes { get; private set; }
        public ICollection<Downvote> Downvotes { get; private set; }
        public ICollection<SavedPost> SavedBy { get; private set; }
        public ICollection<Report> Reports { get; private set; }
        

        private Post()
        {
        }

        public Post(Guid userId, string message, string imageUrl, double latitude, double longitude)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));
            
            UserId = userId;
            Message = message;
            ImageUrl = imageUrl;
            Latitude = latitude;
            Longitude = longitude;
            Deleted = false;

            Upvotes = new List<Upvote>();
            Downvotes = new List<Downvote>();
            SavedBy = new List<SavedPost>();
            Reports = new List<Report>();
        }

        public void Delete()
            => Deleted = true;
    }
}