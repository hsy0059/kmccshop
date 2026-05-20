using Microsoft.EntityFrameworkCore;
using Social.Service.Models.Entities;

namespace Social.Service.Data;

public class SocialDbContext : Campus.Infrastructure.BaseDbContext
{
    public SocialDbContext(DbContextOptions<SocialDbContext> options) : base(options) { }

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();
    public DbSet<PostComment> PostComments => Set<PostComment>();
    public DbSet<SecondGoods> SecondGoods => Set<SecondGoods>();
    public DbSet<LostFound> LostFounds => Set<LostFound>();
    public DbSet<Advertisement> Advertisements => Set<Advertisement>();
}