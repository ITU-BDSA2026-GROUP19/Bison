namespace Bison.Model;

public class Post
{
    public int PostId { get; set; }
    public string Text { get; set; }
    public DateTime TimeStamp { get; set; }
    public Author Author { get; set; }
}