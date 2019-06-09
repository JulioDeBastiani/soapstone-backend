using System.Collections.Generic;

namespace Soapstone.Domain
{
    public class User : Entity
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        public ICollection<Upvote> Upvotes { get; private set; }
        public ICollection<Downvote> Downvotes { get; private set; }
        public ICollection<SavedPost> SavedPosts { get; private set; }
        public ICollection<Report> Reports { get; private set; }

        private User()
        {
        }

        public User(string username, string email, string password)
        {
            Username = username;
            Email = email;
            Password = password; // TODO hash

            Upvotes = new List<Upvote>();
            Downvotes = new List<Downvote>();
            SavedPosts = new List<SavedPost>();
            Reports = new List<Report>();
        }
    }
}