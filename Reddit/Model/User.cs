using System.ComponentModel.DataAnnotations;

namespace Reddit.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        public virtual List<Post> Posts { get; set; } = new List<Post>();
        public virtual List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
