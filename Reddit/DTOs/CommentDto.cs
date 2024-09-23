namespace Reddit.DTOs
{
    public class CommentDto
    {
        public string AuthorName { get; set; }
        public string Content { get; set; }
        public int Upvote { get; set; }
        public int Downvote { get; set; }
        public int PostId { get; set; }
    }
}
