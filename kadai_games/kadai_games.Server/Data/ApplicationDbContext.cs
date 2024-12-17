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

    // 引数なしのデフォルトコンストラクタ（モック用）
    public ApplicationDbContext() { }

    // 新しいテーブルを DbSet として追加
    public virtual DbSet<Maker> Makers { get; set; }
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<Genre> Genres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder); // Identity 用の設定



    }

  }

}

