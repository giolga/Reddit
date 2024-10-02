namespace Reddit.DTOs
{
    public class PostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int AuthorId { get; set; }
        public int CommunityId { get; set; }
    }
}
