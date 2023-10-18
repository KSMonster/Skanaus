namespace Skanaus.Data.Entities;

public class Comment
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime CreationDate { get; set; }
    
    public required Recipe Recipe { get; set; }
}

public record CommentDto(int Id, string Content, DateTime CreationDate, Recipe recipe);
public record CommentAllDto(int Id, string Content, DateTime CreationDate);