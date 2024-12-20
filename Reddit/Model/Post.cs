﻿using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Reddit.Model
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        //[JsonIgnore]
        public virtual List<Comment> Comments { get; set; } = new List<Comment>();
        public int AuthorId { get; set; }
        public virtual User Author { get; set; }


        public int CommunityId { get; set; }
        public virtual Community Community { get; set; }

        //added later
        public int Upvote { get; set; }
        public int Downvote { get; set; }

    }
}
