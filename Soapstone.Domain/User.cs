using System;
using System.Collections.Generic;

namespace Soapstone.Domain
{
    public class User : Entity
    {
        // TODO flair/title?
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        // TODO logic deletion

        public ICollection<Post> Posts { get; private set; }
        public ICollection<Upvote> Upvotes { get; private set; }
        public ICollection<Downvote> Downvotes { get; private set; }
        public ICollection<SavedPost> SavedPosts { get; private set; }
        public ICollection<Report> Reports { get; private set; }

        private User()
        {
        }

        public User(string username, string email, string password)
        {
            // TODO validate username
            Username = username;
            Email = email;
            SetPassword(password);

            Posts = new List<Post>();
            Upvotes = new List<Upvote>();
            Downvotes = new List<Downvote>();
            SavedPosts = new List<SavedPost>();
            Reports = new List<Report>();
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword))
                throw new ArgumentNullException(nameof(oldPassword));

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentNullException(nameof(newPassword));

            // TODO hash
            if (Password != oldPassword)
                throw new ArgumentException(nameof(oldPassword));

            SetPassword(newPassword);
        }

        private void SetPassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentNullException(nameof(newPassword));

            // TODO hash
            Password = newPassword;
        }

        public bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            // TODO hash
            return Password == password;
        }
    }
}