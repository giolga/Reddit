﻿using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;

namespace Reddit.Model
{
    public class Comment
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string Content { get; set; }
        public int Upvote { get; set; }
        public int Downvote { get; set; }
        public int PostId { get; set; }
        [JsonIgnore]
        public Post Post { get; set; }
    }
}
