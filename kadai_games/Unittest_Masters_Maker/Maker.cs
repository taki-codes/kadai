using kadai_games.Data;
using kadai_games.Server.Controllers;
using kadai_games.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Unittest_Masters_Maker
{
  [TestClass]
  public sealed class Unittest_Masters_Maker
  {
    private ApplicationDbContext _context;
    private MakerController _controller;

    [TestInitialize]
    public void TestInitialize()
    {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MyDatabase02;Trusted_Connection=True;MultipleActiveResultSets=true;")
          .Options;

      _context = new ApplicationDbContext(options);
      _controller = new MakerController(_context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
      _context.Dispose();
    }

    /// <summary>
    /// メーカー一覧取得
    /// </summary>
    [TestMethod]
    public void GetMakers_ReturnsAllMakers_WhenNoSearchText()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        _context.Makers.Add(new Maker { Maker_Name = "Sony", Maker_Address = "Tokyo1-1-1", Delete_Flg = false });
        _context.Makers.Add(new Maker { Maker_Name = "Nintendo", Maker_Address = "Kyoto1-1-1", Delete_Flg = false });
        _context.SaveChanges();

        var result = _controller.GetMakers(null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var makers = result.Value as IEnumerable<object>;
        Assert.IsNotNull(makers);
        Assert.IsTrue(makers.Count() == 2);
        Assert.IsTrue(_context.Makers.Any(g => g.Maker_Name == "Sony" && !g.Delete_Flg));
        Assert.IsTrue(_context.Makers.Any(g => g.Maker_Name == "Nintendo" && !g.Delete_Flg));

        transaction.Rollback();
      }
    }

    /// <summary>
    /// メーカー一覧取得(検索条件有)
    /// </summary>
    [TestMethod]
    public void GetMakers_ReturnsFilteredMakers_WhenSearchText()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        _context.Makers.Add(new Maker { Maker_Name = "Sony", Maker_Address = "Tokyo1-1-1", Delete_Flg = false });
        _context.Makers.Add(new Maker { Maker_Name = "Nintendo", Maker_Address = "Kyoto1-1-1", Delete_Flg = false });
        _context.Makers.Add(new Maker { Maker_Name = "SEGA", Maker_Address = "Osaka1-1-1", Delete_Flg = false });
        _context.SaveChanges();
        string searchText = "S";

        // Act
        var result = _controller.GetMakers(searchText) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var makers = result.Value as IEnumerable<object>;
        Assert.IsNotNull(makers, "Makers should not be null.");
        Assert.AreEqual(2, makers.Count(), "Only makers containing 'S' should be returned.");
        Assert.IsFalse(makers.Any(m => m.GetType().GetProperty("Maker_Name").GetValue(m).ToString() == "Nintendo"));

        transaction.Rollback();
      }
    }

    /// <summary>
    /// メーカー詳細取得
    /// </summary>
    [TestMethod]
    public void GetMakerDetails_ReturnsMaker_WhenIdExists()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        var maker = new Maker { Maker_Name = "Sony", Maker_Address = "Tokyo1-1-1", Delete_Flg = false };
        _context.Makers.Add(maker);
        _context.SaveChanges();

        var result = _controller.GetMakerDetails(maker.Maker_Id) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var returnedMaker = result.Value as Maker;
        Assert.IsNotNull(returnedMaker);
        Assert.AreEqual("Sony", returnedMaker.Maker_Name);

        transaction.Rollback();
      }
    }

    /// <summary>
    /// メーカー新規作成
    /// </summary>
    [TestMethod]
    public void CreateMaker_TransactionRollback_AddsMaker()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        var makerModel = new MakerViewModel { Maker_Name = "Nintendo", Maker_Address = "Tokyo1-1-1" };

        var result = _controller.CreateMaker(makerModel) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var createdMaker = _context.Makers.FirstOrDefault(m => m.Maker_Name == "Nintendo");
        Assert.IsNotNull(createdMaker);
        Assert.AreEqual("Nintendo", createdMaker.Maker_Name);
        Assert.AreEqual("Tokyo1-1-1", createdMaker.Maker_Address);
        Assert.AreEqual(false, createdMaker.Delete_Flg);
        // データベースにデータが存在することを再確認
        Assert.IsTrue(_context.Makers.Any(g => g.Maker_Name == "Nintendo"));

        transaction.Rollback();
      }
    }

    /// <summary>
    /// メーカー更新
    /// </summary>
    [TestMethod]
    public void UpdateMaker_SuccessfullyUpdatesMaker_WhenNoConflict()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        var maker = new Maker { Maker_Name = "Sony", Maker_Address = "Tokyo1-1-1", Delete_Flg = false };
        _context.Makers.Add(maker);
        _context.SaveChanges();

        var updatedMakerModel = new MakerViewModel { Maker_Name = "SEGA", Maker_Address = "Osaka1-1-1" };

        var result = _controller.UpdateMaker(maker.Maker_Id, updatedMakerModel) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var updatedMaker = _context.Makers.FirstOrDefault(m => m.Maker_Id == maker.Maker_Id);
        Assert.IsNotNull(updatedMaker);
        Assert.AreEqual("SEGA", updatedMaker.Maker_Name);
        Assert.AreEqual("Osaka1-1-1", updatedMaker.Maker_Address);

        transaction.Rollback();
      }
    }

    /// <summary>
    /// メーカー削除
    /// </summary>
    [TestMethod]
    public void DeleteMaker_TransactionRollback_SetsDeleteFlag()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        var maker = new Maker { Maker_Name = "Konami", Maker_Address = "Osaka1-1-1", Delete_Flg = false };
        _context.Makers.Add(maker);
        _context.SaveChanges();

        var result = _controller.DeleteMaker(maker.Maker_Id) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var deletedMaker = _context.Makers.Find(maker.Maker_Id);
        Assert.IsTrue(deletedMaker.Delete_Flg);

        transaction.Rollback();
      }
    }

    /// <summary>
    /// 重複メーカー名での作成エラーチェック
    /// </summary>
    [TestMethod]
    public void CreateMaker_ReturnsBadRequest_WhenMakerNameAlreadyExists()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        var existingMaker = new Maker { Maker_Name = "Sega", Maker_Address = "Tokyo1-1-1", Delete_Flg = false };
        _context.Makers.Add(existingMaker);
        _context.SaveChanges();

        var newMakerModel = new MakerViewModel { Maker_Name = "Sega", Maker_Address = "Osaka1-1-1" };

        var result = _controller.CreateMaker(newMakerModel) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        var response = result.Value.GetType().GetProperty("Message").GetValue(result.Value, null);
        Assert.AreEqual("同じメーカー名が既に存在します。", response);

        transaction.Rollback();
      }
    }

    /// <summary>
    /// ゲーム一覧にデータが存在する場合の削除エラーチェック
    /// </summary>
    [TestMethod]
    public void DeleteMaker_ReturnsBadRequest_WhenGamesExist()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        // 必要なジャンルを追加
        var genre = new Genre { Genre_Name = "RPG", Delete_Flg = false };
        _context.Genres.Add(genre);
        _context.SaveChanges();

        // メーカーを追加
        var maker = new Maker { Maker_Name = "Bandai", Maker_Address = "Tokyo", Delete_Flg = false };
        _context.Makers.Add(maker);
        _context.SaveChanges();

        // ジャンルに関連付けられたゲームを追加
        var game = new Game
        {
          Title = "Test Game",
          Genre_Id = genre.Genre_Id,  // 外部キーエラーを防ぐために関連付け
          Maker_Id = maker.Maker_Id,
          Delete_Flg = false
        };
        _context.Games.Add(game);
        _context.SaveChanges();

        // Act
        var result = _controller.DeleteMaker(maker.Maker_Id) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        var response = result.Value.GetType().GetProperty("Message").GetValue(result.Value, null);
        Assert.AreEqual("このメーカーはゲーム一覧に存在するため削除できません。", response);

        transaction.Rollback();
      }
    }

    /// <summary>
    /// 重複メーカー名での更新エラーチェック
    /// </summary>
    [TestMethod]
    public void UpdateMaker_ReturnsBadRequest_WhenMakerNameAlreadyExists()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        var maker1 = new Maker { Maker_Name = "Sony", Maker_Address = "Tokyo1-1-1", Delete_Flg = false };
        var maker2 = new Maker { Maker_Name = "Nintendo", Maker_Address = "Kyoto1-1-1", Delete_Flg = false };
        _context.Makers.AddRange(maker1, maker2);
        _context.SaveChanges();

        var updatedMakerModel = new MakerViewModel { Maker_Name = "Sony", Maker_Address = "Osaka" };

        var result = _controller.UpdateMaker(maker2.Maker_Id, updatedMakerModel) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        var response = result.Value.GetType().GetProperty("Message").GetValue(result.Value, null);
        Assert.AreEqual("同じメーカー名が既に存在します。", response);

        transaction.Rollback();
      }
    }
  }
}
