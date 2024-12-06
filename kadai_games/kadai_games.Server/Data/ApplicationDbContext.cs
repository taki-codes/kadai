using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using kadai_games.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
namespace kadai_games.Data
{
  public class ApplicationDbContext : IdentityDbContext<Users>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    // 新しいテーブルを DbSet として追加
    public DbSet<Maker> Makers { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Genre> Genres { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder); // Identity 用の設定

    }
  }
}

