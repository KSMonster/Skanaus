using System.ComponentModel.DataAnnotations;
using Skanaus.Auth.Model;

namespace Skanaus.Data.Entities;

public class Comment
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime CreationDate { get; set; }
    
    public required Recipe Recipe { get; set; }
    [Required]
    public required string UserId { get; set; }
    public ForumRestUser User { get; set; }
}

public record CommentDto(int Id, string Content, DateTime CreationDate, Recipe recipe);
public record CommentAllDto(int Id, string Content, DateTime CreationDate);