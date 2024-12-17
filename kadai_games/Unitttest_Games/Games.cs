using kadai_games.Controllers;
using kadai_games.Data;
using kadai_games.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameControllerTests
{
  [TestClass]
  public class GameControllerTransactionTests
  {
    private ApplicationDbContext _context;
    private GameController _controller;

    // 自動生成された ID を保持するフィールド
    private int TestGenreId;
    private int TestMakerId;

    /// <summary>
    /// テストの初期化: 実データベースのセットアップ
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MyDatabase;Trusted_Connection=True;MultipleActiveResultSets=true;")
          .Options;

      _context = new ApplicationDbContext(options);
      _controller = new GameController(_context);

      // ジャンルが存在しない場合のみ追加
      if (!_context.Genres.Any(g => g.Genre_Name == "Action"))
      {
        var genre = new Genre { Genre_Name = "Action" };
        _context.Genres.Add(genre);
        _context.SaveChanges();
        TestGenreId = genre.Genre_Id;
      }
      else
      {
        TestGenreId = _context.Genres.First(g => g.Genre_Name == "Action").Genre_Id;
      }

      // メーカーが存在しない場合のみ追加
      if (!_context.Makers.Any(m => m.Maker_Name == "Nintendo"))
      {
        var maker = new Maker { Maker_Name = "Nintendo" };
        _context.Makers.Add(maker);
        _context.SaveChanges();
        TestMakerId = maker.Maker_Id;
      }
      else
      {
        TestMakerId = _context.Makers.First(m => m.Maker_Name == "Nintendo").Maker_Id;
      }
    }

    /// <summary>
    /// テスト終了後にリソースを解放
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
      _context.Dispose();
    }

    /// <summary>
    /// 新規作成で正常に作成された場合、OK が返されることをテスト
    /// </summary>
    [TestMethod]
    public void CreateGameReturnsOkWhenGameIsCreatedSuccessfully()
    {
      // Arrange: テスト用のタイトルを変数に格納
      var title = Guid.NewGuid().ToString();
      var memo = Guid.NewGuid().ToString();
      using (var transaction = _context.Database.BeginTransaction())
      {


        var model = new GameViewModel
        {
          Title = title,
          Maker_Id = TestMakerId, // 自動生成された ID を使用
          Genre_Id = TestGenreId, // 自動生成された ID を使用
          Sales_Count = 100,
          Memo = memo
        };

        // Act: メソッドを呼び出す
        var result = _controller.CreateGame(model) as OkObjectResult;

        // Assert: 結果を検証
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(200, result.StatusCode, "Status code should be 200.");

        // データベースに新規作成された内容を検証
        var createdGame = _context.Games.FirstOrDefault(g => g.Title == title);
        Assert.IsNotNull(createdGame, "Created game should not be null.");
        Assert.AreEqual(title, createdGame.Title, "Game title should match.");
        Assert.AreEqual(TestMakerId, createdGame.Maker_Id, "Maker_Id should match.");
        Assert.AreEqual(TestGenreId, createdGame.Genre_Id, "Genre_Id should match.");
        Assert.AreEqual(100, createdGame.Sales_Count, "Sales_Count should match.");
        Assert.AreEqual(memo, createdGame.Memo, "Memo should match.");

        Assert.IsTrue(_context.Games.Any(g => g.Title == title));

        // トランザクションをロールバック
        transaction.Rollback();
      }
    }

    /// <summary>
    /// 有効な ID で詳細情報を取得した場合、200 OK とゲームデータが返されることを確認
    /// </summary>
    [TestMethod]
    public void GetGame_ReturnsOkResult_WithValidId()
    {
      var title = Guid.NewGuid().ToString();
      var memo = Guid.NewGuid().ToString();

      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange: ジャンルとメーカーを追加
        var genre = new Genre { Genre_Name = "Action" };
        var maker = new Maker { Maker_Name = "Nintendo" };

        _context.Genres.Add(genre);
        _context.Makers.Add(maker);

        // 保存
        _context.SaveChanges();

        // Arrange: ゲームデータを追加
        var game = new Game
        {
          Title = title,
          Genre_Id = genre.Genre_Id, // 外部キーとして追加したジャンルを参照
          Maker_Id = maker.Maker_Id, // 外部キーとして追加したメーカーを参照
          Delete_Flg = false
        };

        _context.Games.Add(game);

        // 保存
        _context.SaveChanges();

        // Act
        var result = _controller.GetGame(game.Game_Id) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(200, result.StatusCode, "Status code should be 200.");
        Assert.IsTrue(_context.Games.Any(g => g.Title == title));

        // Rollback
        transaction.Rollback();
      }
    }

    /// <summary>
    /// 有効な ID で詳細情報を更新した場合、200 OK と更新されたデータが返されることを確認
    /// </summary>
    [TestMethod]
    public void UpdateGame_UpdatesAndReturnsGame_WhenValidId()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange: 必要なデータを準備
        var genre1 = new Genre { Genre_Name = "Action" };
        var genre2 = new Genre { Genre_Name = "Adventure" };
        var maker1 = new Maker { Maker_Name = "Nintendo" };
        var maker2 = new Maker { Maker_Name = "Sony" };

        _context.Genres.AddRange(genre1, genre2);
        _context.Makers.AddRange(maker1, maker2);
        _context.SaveChanges();

        var game = new Game
        {
          Title = "Old Title",
          Maker_Id = maker1.Maker_Id,
          Genre_Id = genre1.Genre_Id,
          Sales_Count = 100,
          Memo = "Old Memo",
          Delete_Flg = false
        };

        _context.Games.Add(game);
        _context.SaveChanges();

        var updatedGame = new Game
        {
          Title = "New Title",
          Maker_Id = maker2.Maker_Id,
          Genre_Id = genre2.Genre_Id,
          Sales_Count = 200,
          Memo = "New Memo"
        };

        // Act
        var result = _controller.UpdateGame(game.Game_Id, updatedGame) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(200, result.StatusCode);

        var updatedResult = result.Value as Game;
        Assert.IsNotNull(updatedResult, "Updated game should not be null.");
        Assert.AreEqual("New Title", updatedResult.Title);
        Assert.AreEqual(maker2.Maker_Id, updatedResult.Maker_Id);
        Assert.AreEqual(genre2.Genre_Id, updatedResult.Genre_Id);
        Assert.AreEqual(200, updatedResult.Sales_Count);
        Assert.AreEqual("New Memo", updatedResult.Memo);

        // Rollback
        transaction.Rollback();
      }
    }


    /// <summary>
    /// 有効な ID で詳細情報を削除した場合、200 OK が返されることを確認
    /// </summary>
    [TestMethod]
    public void DeleteGame_ReturnsOkResult_WhenGameExists()
    {
      var title = Guid.NewGuid().ToString();
      var memo = Guid.NewGuid().ToString();

      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        var genre = new Genre { Genre_Name = "Action" };
        var maker = new Maker { Maker_Name = "Nintendo" };

        _context.Genres.Add(genre);
        _context.Makers.Add(maker);
        _context.SaveChanges();

        var game = new Game
        {
          Title = title,
          Genre_Id = genre.Genre_Id,
          Maker_Id = maker.Maker_Id,
          Delete_Flg = false
        };

        _context.Games.Add(game);
        _context.SaveChanges();

        // Act
        var result = _controller.DeleteGame(game.Game_Id) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(200, result.StatusCode);

        var responseMessage = result.Value as SuccessResponse;
        Assert.IsNotNull(responseMessage, "Response message should not be null.");
        Assert.AreEqual("ゲームを削除しました。", responseMessage.Message);

        Assert.IsTrue(game.Delete_Flg, "Delete_Flg should be set to true.");

        // Rollback
        transaction.Rollback();
      }
    }

    /// <summary>
    /// ジャンル一覧取得時に 200 OK と期待されるジャンルリストが返されることを確認
    /// </summary>
    [TestMethod]
    public void GetGenres_ShouldReturnOkResult_WithGenresList()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        _context.Genres.RemoveRange(_context.Genres);
        _context.SaveChanges();

        // Arrange
        _context.Genres.AddRange(
            new Genre { Genre_Name = "Adventure" },
            new Genre { Genre_Name = "RPG" }
        );
        _context.SaveChanges();

        // Act
        var result = _controller.GetGenres() as OkObjectResult;

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(200, result.StatusCode, "Status code should be 200.");
        var genres = result.Value as IEnumerable<dynamic>;
        Assert.IsNotNull(genres, "Genres should not be null.");
        Assert.AreEqual(2, genres.Count(), "Makers count should match.");

        transaction.Rollback();
      }
    }

    /// <summary>
    /// メーカー一覧取得時に 200 OK と期待されるメーカーリストが返されることを確認
    /// </summary>
    [TestMethod]
    public void GetMakers_ShouldReturnOkResult_WithMakersList()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange: 既存データをクリーンアップ
        _context.Makers.RemoveRange(_context.Makers);
        _context.SaveChanges();

        // Arrange: テスト用データを追加
        _context.Makers.AddRange(
            new Maker { Maker_Name = "Test Maker1", Maker_Address = "Test Address1" },
            new Maker { Maker_Name = "Test Maker2", Maker_Address = "Test Address2" }
        );
        _context.SaveChanges();

        // Act: メーカー一覧を取得
        var result = _controller.GetMakers() as OkObjectResult;

        // Assert: 結果を検証
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(200, result.StatusCode, "Status code should be 200.");
        var makers = result.Value as IEnumerable<dynamic>;
        Assert.IsNotNull(makers, "Makers should not be null.");
        Assert.AreEqual(2, makers.Count(), "Makers count should match.");

        // テストデータをロールバック
        transaction.Rollback();
      }
    }

    /// <summary>
    /// モデルが無効な場合、BadRequest が返されることをテスト
    /// </summary>
    [TestMethod]
    public void CreateGame_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
      var title = Guid.NewGuid().ToString();
      var memo = Guid.NewGuid().ToString();
      // Arrange: 無効なデータモデルを準備
      var model = new GameViewModel
      {
        Title = "", // タイトルが空
        Maker_Id = TestMakerId,
        Genre_Id = TestGenreId,
        Sales_Count = 10,
        Memo = memo
      };

      // モデルステートにエラーを追加
      _controller.ModelState.AddModelError("Title", "Title is required.");

      // Act: メソッドを呼び出す
      var result = _controller.CreateGame(model) as BadRequestObjectResult;

      // Assert: 結果を検証
      Assert.IsNotNull(result, "Result should not be null.");
      Assert.AreEqual(400, result.StatusCode, "Status code should be 400.");

      // エラー応答を検証
      var errorResponse = result.Value as Dictionary<string, string[]>;
      Assert.IsNotNull(errorResponse, "Error response should not be null.");
      Assert.AreEqual("Title is required.", errorResponse["Title"].First(), "Error message should match.");
    }

    /// <summary>
    /// 無効な Genre_Id または Maker_Id の場合、BadRequest が返されることをテスト
    /// </summary>
    [TestMethod]
    public void CreateGame_ReturnsBadRequest_WhenGenreOrMakerIsInvalid()
    {
      var title = Guid.NewGuid().ToString();
      var memo = Guid.NewGuid().ToString();
      // Arrange: 無効な Maker_Id と Genre_Id を設定
      var model = new GameViewModel
      {
        Title = title,
        Maker_Id = 99, // 存在しない Maker_Id
        Genre_Id = 99, // 存在しない Genre_Id
        Sales_Count = 10,
        Memo = memo
      };

      // Act: メソッドを呼び出す
      var result = _controller.CreateGame(model) as BadRequestObjectResult;

      // Assert: 結果を検証
      Assert.IsNotNull(result, "Result should not be null.");
      Assert.AreEqual(400, result.StatusCode, "Status code should be 400.");

      // エラー応答を検証
      var errorResponse = result.Value as ErrorResponse;
      Assert.IsNotNull(errorResponse, "Error response should not be null.");
      Assert.AreEqual("無効な Genre_Id または Maker_Id です。", errorResponse.Message, "Error message should match.");

    }


  }

}


