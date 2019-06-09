using System;

namespace Soapstone.Domain
{
    public class Downvote : Entity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public Guid PostId { get; private set; }
        public Post Post { get; private set; }

        private Downvote()
        {
        }

        public Downvote(Guid userId, Guid postId)
        {
            UserId = userId;
            PostId = postId;
        }
    }
}