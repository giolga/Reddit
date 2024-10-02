namespace Reddit.DTOs
{
    public class CommentDto
    {
        public string Content { get; set; }
        public int PostId { get; set; }
        public int AuthorId{ get; set; }
    }
}