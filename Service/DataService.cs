using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Data;
using Model;

namespace Service;

public class DataService
{
    private DataContext db { get; }

    public DataService(DataContext db)
    {
        this.db = db;
    }

    public void SeedData()
    {
        if (!db.Posts.Any())
        {
            var user1 = new User { Username = "Kristian" };
            var user2 = new User { Username = "Søren" };
            var user3 = new User { Username = "Pernille" };
            var user4 = new User { Username = "Alex" };
            var user5 = new User { Username = "Signe" };
            var user6 = new User { Username = "Hugo" };
            var user7 = new User { Username = "Marianne" };
            var user8 = new User { Username = "Kevin" };
            var user9 = new User { Username = "Isabella" };
            var user10 = new User { Username = "Keld" };
            

            db.Users.AddRange(user1, user2, user3, user4, user5, user6, user7, user8, user8, user9, user10);

            var currentTime = DateTime.Now;

            var post1 = new Post { Title = "Fodbold nyt", Content = "Ronaldo laver Hattrick i Suadi", User = user1, Upvotes = 30, Downvotes = 45, CreationDate = currentTime };
            var post2 = new Post { Title = "Argentina vinder VM", Content = "Messi is the GOAT", User = user2, Upvotes = 342, Downvotes = 333, CreationDate = currentTime };
            var post3 = new Post { Title = "Manchester City News", Content = "Den norske viking er ankommet", User = user3, Upvotes = 8, Downvotes = 1, CreationDate = currentTime };
            var post4 = new Post { Title = "The red devils", Content = "Erik Ten Hag Sacked in the morning", User = user4, Upvotes = 500, Downvotes = 100, CreationDate = currentTime };
            var post5 = new Post { Title = "Skal folk i byen?", Content = "Det jo fredag", User = user5, Upvotes = 50, Downvotes = 2, CreationDate = currentTime };
            var post6 = new Post { Title = "Læsning er sundt", Content = "Du bliver klogere af at læse", User = user6, Upvotes = 5, Downvotes = 4, CreationDate = currentTime };
            var post7 = new Post { Title = "Bedste senge 2024", Content = "Her en en liste over top 10 senge", User = user7, Upvotes = 690, Downvotes = 400, CreationDate = currentTime };
            var post8 = new Post { Title = "Keder du dig?", Content = "Så lav det her i stedet", User = user8, Upvotes = 550, Downvotes = 2, CreationDate = currentTime };
            var post9 = new Post { Title = "Nova nordisk", Content = "Nova nordisk præsentere nyt slankemiddel", User = user9, Upvotes = 900, Downvotes = 800, CreationDate = currentTime };
            var post10 = new Post { Title = "Apple Breaking news", Content = "Apple går konkurs", User = user10, Upvotes = 56, Downvotes = 50, CreationDate = currentTime };

            post1.Comments.Add(new Comment { Content = "Han er bare god", User = user2, Upvotes = 5, Downvotes = 1, CreationDate = currentTime });
            post2.Comments.Add(new Comment { Content = "The greatest of alle time", User = user4, Upvotes = 7, Downvotes = 3, CreationDate = currentTime });
            post3.Comments.Add(new Comment { Content = "Erling Haaland", User = user5, Upvotes = 3, Downvotes = 0, CreationDate = currentTime });
            post4.Comments.Add(new Comment { Content = "United er piv ringe", User = user3, Upvotes = 2, Downvotes = 1, CreationDate = currentTime });
            post5.Comments.Add(new Comment { Content = "Ja og Danmark vinder EM 2024", User = user1, Upvotes = 4, Downvotes = 2, CreationDate = currentTime });
            post6.Comments.Add(new Comment { Content = "Men det også kedeligt", User = user7, Upvotes = 9, Downvotes = 2, CreationDate = currentTime });
            post7.Comments.Add(new Comment { Content = "Min seng er en Bambus seng", User = user6, Upvotes = 344, Downvotes = 254, CreationDate = currentTime });
            post8.Comments.Add(new Comment { Content = "Det kedeligt at kede sig", User = user10, Upvotes = 34, Downvotes = 72, CreationDate = currentTime });
            post9.Comments.Add(new Comment { Content = "Det skal jeg have, jeg er tyk", User = user8, Upvotes = 43, Downvotes = 255, CreationDate = currentTime });
            post10.Comments.Add(new Comment { Content = "Lorte iphone", User = user7, Upvotes = 555, Downvotes = 20, CreationDate = currentTime });

            db.Posts.AddRange(post1, post2, post3, post4, post5, post6, post7, post8, post9, post10);

            db.SaveChanges();
        }
    }


    public List<Post> GetPosts()
{
    return db.Posts.Include(p => p.User).ToList();
}

public Post GetPost(int id)
{
    return db.Posts.Include(p => p.User).FirstOrDefault(p => p.Id == id);
}

public List<User> GetAuthors()
{
    return db.Users.ToList();
}

public User GetAuthor(int id)
{
    return db.Users.Include(u => u.Posts).FirstOrDefault(u => u.Id == id);
}

public async Task<Post> UpvotePost(int id)
{
    var post = await db.Posts.FindAsync(id);
    if (post != null)
    {
        post.Upvotes++;
        await db.SaveChangesAsync();
    }
    return post;
}

public async Task<Post> DownvotePost(int id)
{
    var post = await db.Posts.FindAsync(id);
    if (post != null)
    {
        post.Downvotes++;
        await db.SaveChangesAsync();
    }
    return post;
}

public async Task<Comment> UpvoteComment(int id)
{
    var comment = await db.Comments.FindAsync(id);
    if (comment != null)
    {
        comment.Upvotes++;
        await   db.SaveChangesAsync();
    }
    return comment;
}

public async Task<Comment> DownvoteComment(int id)
{
    var comment = await db.Comments.FindAsync(id);
    if (comment != null)
    {
        comment.Downvotes++;
        await db.SaveChangesAsync();
    }
    return comment;
}

public string CreatePost(string title, string content, int userId)
{
    var user = db.Users.FirstOrDefault(u => u.Id == userId);
    if (user == null)
        return "User not found";

        var currentTime = DateTime.Now;

        db.Posts.Add(new Post { Title = title, Content = content, User = user, CreationDate = currentTime });
    db.SaveChanges();
    return "Post created";
}

public async Task<Comment> CreateComment(string content, int postId, int userId)
{
    var post = await db.Posts.FindAsync(postId);
        if (post == null)
        {
            // Håndter fejl, hvis posten ikke blev fundet
            return null;
        }

        var currentTime = DateTime.Now;

        var comment = new Comment { Content = content, UserId = userId, CreationDate = currentTime };
    post.Comments.Add(comment);
    await db.SaveChangesAsync();

    return comment;
}

}
