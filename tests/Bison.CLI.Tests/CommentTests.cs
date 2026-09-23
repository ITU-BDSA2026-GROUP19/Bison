using Xunit;
using Bison.Cheep;
using SimpleDB;
namespace Bison.CLI.Tests;

public class CommentTests
{
    [Fact]
    public void Comment_stored()
    {
        // Arrange
        const string filename = "test_comments.csv";

        if (File.Exists(filename))
        {
            File.Delete(filename);
        }

        var commentsDatabase = CSVDatabase<Comment>.GetInstance(filename);
        var comment = new Comment(1, "test", "user", 1234567890);

        // Act
        commentsDatabase.Store(comment);

        //assert
        var storedComments = commentsDatabase.Read();
        var storedComment = storedComments.First();
        Assert.True(storedComment != null);
        Assert.Single(storedComments);
        Assert.Equal(comment.ObservationId, storedComment.ObservationId);
        Assert.Equal(comment.Author, storedComment.Author);
        Assert.Equal(comment.Message, storedComment.Message);
        Assert.Equal(comment.Timestamp, storedComment.Timestamp);
    }
}