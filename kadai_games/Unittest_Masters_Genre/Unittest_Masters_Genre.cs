using kadai_games.Data;
using kadai_games.Server.Controllers;
using kadai_games.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Unittest_Masters_Genre
{
  [TestClass]
  public sealed class Unittest_Masters_Genre
  {
    private ApplicationDbContext _context;
    private GenreController _controller;

    [TestInitialize]
    public void TestInitialize()
    {
      // データベースオプションの設定（SQL Server LocalDB使用）
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MyDatabase;Trusted_Connection=True;MultipleActiveResultSets=true;")
          .Options;

      _context = new ApplicationDbContext(options);
      _controller = new GenreController(_context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
      _context.Dispose();
    }

    /// <summary>
    /// ジャンル一覧取得
    /// </summary>
    [TestMethod]
    public void GetGenres_ReturnsAllGenres_WhenNoSearchText()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        _context.Genres.Add(new Genre { Genre_Name = "RPG", Delete_Flg = false });
        _context.Genres.Add(new Genre { Genre_Name = "Action", Delete_Flg = false });
        _context.SaveChanges();

        // Act
        var result = _controller.GetGenres(null) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var genres = result.Value as IEnumerable<object>;
        Assert.IsNotNull(genres, "Genres should not be null.");
        Assert.IsTrue(_context.Genres.Any(g => g.Genre_Name == "RPG" && !g.Delete_Flg));
        Assert.IsTrue(_context.Genres.Any(g => g.Genre_Name == "Action" && !g.Delete_Flg));

        transaction.Rollback();
      }
    }

    /// <summary>
    /// ジャンル詳細取得
    /// </summary>
    [TestMethod]
    public void GetGenreDetails_ReturnsGenre_WhenIdExists()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        var genre = new Genre { Genre_Name = "RPG", Delete_Flg = false };
        _context.Genres.Add(genre);
        _context.SaveChanges();

        // Act
        var result = _controller.GetGenreDetails(genre.Genre_Id) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var returnedGenre = result.Value as Genre;
        Assert.IsNotNull(returnedGenre);
        Assert.AreEqual("RPG", returnedGenre.Genre_Name);

        transaction.Rollback();
      }
    }
    /// <summary>
    /// ジャンル新規作成
    /// </summary>
    [TestMethod]
    public void CreateGenre_TransactionRollback_AddsGenre()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        var genreModel = new GenreViewModel { Genre_Name = "Adventure" };

        // Act
        var result = _controller.CreateGenre(genreModel) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        // データベースに新規作成された内容を検証
        var createdGenre = _context.Genres.FirstOrDefault(g => g.Genre_Name == "Adventure");
        Assert.IsNotNull(createdGenre, "Created genre should not be null.");
        Assert.AreEqual("Adventure", createdGenre.Genre_Name, "Genre name should match.");
        Assert.AreEqual(false, createdGenre.Delete_Flg, "Delete_Flg should be false.");
        Assert.AreEqual("admin", createdGenre.CreatedUser, "CreatedUser should match.");
        Assert.IsTrue(createdGenre.CreateDate <= DateTime.Now, "CreateDate should be set correctly.");

        // データベースにデータが存在することを再確認
        Assert.IsTrue(_context.Genres.Any(g => g.Genre_Name == "Adventure"));

        // トランザクションをロールバック
        transaction.Rollback();

      }
    }

    /// <summary>
    /// ジャンル削除
    /// </summary>
    [TestMethod]
    public void DeleteGenre_TransactionRollback_SetsDeleteFlag()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        var genre = new Genre { Genre_Name = "Fantasy", Delete_Flg = false };
        _context.Genres.Add(genre);
        _context.SaveChanges();

        // Act
        var result = _controller.DeleteGenre(genre.Genre_Id) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var deletedGenre = _context.Genres.Find(genre.Genre_Id);
        Assert.IsTrue(deletedGenre.Delete_Flg, "Delete_Flg should be set to true.");

        // トランザクションをロールバック
        transaction.Rollback();

      }
    }

    /// <summary>
    /// ジャンル更新
    /// </summary>
    [TestMethod]
    public void UpdateGenre_TransactionRollback_UpdatesGenre()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        var genre = new Genre { Genre_Name = "RPG" };
        _context.Genres.Add(genre);
        _context.SaveChanges();

        var updatedGenre = new Genre { Genre_Name = "Action" };

        // Act
        var result = _controller.UpdateGenre(genre.Genre_Id, updatedGenre) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var updatedResult = result.Value as Genre;
        Assert.IsNotNull(updatedResult, "Updated genre should not be null.");
        Assert.AreEqual("Action", updatedResult.Genre_Name, "Genre name should be updated.");

        // トランザクションをロールバック
        transaction.Rollback();
      }
    }

    /// <summary>
    /// 重複ジャンル名での作成エラーチェック
    /// </summary>
    [TestMethod]
    public void CreateGenre_ReturnsBadRequest_WhenGenreNameAlreadyExists()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        var existingGenre = new Genre
        {
          Genre_Name = "Action",
          Delete_Flg = false
        };
        _context.Genres.Add(existingGenre);
        _context.SaveChanges();

        var newGenreModel = new GenreViewModel
        {
          Genre_Name = "Action" // 重複するジャンル名
        };

        // Act
        var result = _controller.CreateGenre(newGenreModel) as BadRequestObjectResult;

        // Assert: BadRequestが返されているか確認
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(400, result.StatusCode, "Status code should be 400 BadRequest.");
        var response = result.Value.GetType().GetProperty("Message").GetValue(result.Value, null);
        Assert.AreEqual("同じジャンル名が既に存在します。", response);


        transaction.Rollback();
      }
    }

    /// <summary>
    /// ゲーム一覧にデータが存在する場合の削除エラーチェック
    /// </summary>
    [TestMethod]
    public void DeleteGenre_ReturnsBadRequest_WhenGamesExist()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        // Makersテーブルに関連するデータを追加
        var maker = new Maker
        {
          Maker_Name = "Test Maker"
        };
        _context.Makers.Add(maker);
        _context.SaveChanges();

        // ジャンルを追加
        var genre = new Genre
        {
          Genre_Name = "RPG",
          Delete_Flg = false
        };
        _context.Genres.Add(genre);
        _context.SaveChanges();

        // ジャンルに関連付けられたゲームを追加（外部キーの整合性維持）
        var game = new Game
        {
          Title = "Test Game",
          Genre_Id = genre.Genre_Id,
          Maker_Id = maker.Maker_Id,
          Delete_Flg = false
        };
        _context.Games.Add(game);
        _context.SaveChanges();

        // Act
        var result = _controller.DeleteGenre(genre.Genre_Id) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(400, result.StatusCode, "Status code should be 400 BadRequest.");

        // リフレクションを使用して Message プロパティを取得
        var response = result.Value.GetType().GetProperty("Message").GetValue(result.Value, null);
        Assert.AreEqual("このジャンルはゲーム一覧に存在するため削除できません。", response);

        transaction.Rollback();
      }
    }

    /// <summary>
    /// 重複ジャンル名での更新エラーチェック
    /// </summary>
    [TestMethod]
    public void UpdateGenre_ReturnsBadRequest_WhenGenreNameAlreadyExists()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        // 既存のジャンル
        var genre1 = new Genre { Genre_Name = "Action", Delete_Flg = false };
        var genre2 = new Genre { Genre_Name = "Adventure", Delete_Flg = false };
        _context.Genres.AddRange(genre1, genre2);
        _context.SaveChanges();

        // 更新リクエスト: genre2 を "Action" に変更しようとする（重複）
        var updatedGenre = new Genre { Genre_Name = "Action" };

        // Act
        var result = _controller.UpdateGenre(genre2.Genre_Id, updatedGenre) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(400, result.StatusCode, "Status code should be 400 BadRequest.");

        // リフレクションで Message プロパティを取得
        var response = result.Value.GetType().GetProperty("Message").GetValue(result.Value, null);
        Assert.AreEqual("同じジャンル名が既に存在します。", response);

        transaction.Rollback();
      }
    }
  }
}
