using System.ComponentModel.DataAnnotations;

namespace Reddit.Model
{
    public class Community
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int OwnerId { get; set; }
        public virtual User Owner { get; set; }
        public virtual List<User> Subscribers { get; set; }
        public virtual List<Post> Posts { get; set; }

    }
}
