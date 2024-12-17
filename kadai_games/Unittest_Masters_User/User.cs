using kadai_games.Controllers;
using kadai_games.Data;
using kadai_games.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using static kadai_games.Controllers.UserController;

namespace UserControllerTests
{
  [TestClass]
  public class UserControllerTransactionTests
  {
    private ApplicationDbContext _context;
    private UserController _controller;
    private UserManager<Users> _userManager;

    [TestInitialize]
    public void TestInitialize()
    {
      // データベースオプションの設定
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MyDatabase;Trusted_Connection=True;MultipleActiveResultSets=true;")
          .Options;

      // データベースコンテキストを作成
      _context = new ApplicationDbContext(options);

      // UserManager の依存関係を設定
      var userStore = new UserStore<Users>(_context);
      var mockPasswordHasher = new PasswordHasher<Users>();
      var mockUserValidators = new List<IUserValidator<Users>> { new UserValidator<Users>() };
      var mockPasswordValidators = new List<IPasswordValidator<Users>> { new PasswordValidator<Users>() };
      var mockLookupNormalizer = new UpperInvariantLookupNormalizer();
      var mockErrorDescriber = new IdentityErrorDescriber();
      var mockServices = new ServiceCollection().BuildServiceProvider();
      var mockLogger = new Mock<ILogger<UserManager<Users>>>().Object;

      _userManager = new UserManager<Users>(
           userStore,
           null,
           mockPasswordHasher,
           mockUserValidators,
           mockPasswordValidators,
           mockLookupNormalizer,
           mockErrorDescriber,
           mockServices,
           mockLogger
       );


      _controller = new UserController(_context, _userManager);
    }

    [TestCleanup]
    public void TestCleanup()
    {
      _context.Dispose();
    }

    /// <summary>
    /// ユーザー一覧を取得するテスト
    /// 成功時に 200 OK と正しいデータが返ることを検証
    /// </summary>
    [TestMethod]
    public async Task GetUsers_ReturnsOkResult_WithUsersList()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        var user = new Users
        {
          UserName = "TestUser",
          Email = "test@example.com",
          Is_Admin_Flg = false,
          Delete_Flg = false
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = _controller.GetUsers(null, null) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(200, result.StatusCode, "Status code should be 200.");
        Assert.IsTrue(_context.Users.Any(u => u.UserName == "TestUser"), "Test user should be in the list.");

        transaction.Rollback(); // トランザクションをロールバック
      }
    }

    /// <summary>
    /// 指定されたユーザーIDでユーザー詳細を取得するテスト
    /// 成功時に 200 OK とユーザー情報が返ることを検証
    /// </summary>
    [TestMethod]
    public async Task GetUserDetails_ReturnsOkResult_WhenUserExists()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange: テストデータの作成
        var user = new Users
        {
          Id = "TestUserId",
          UserName = "TestUser",
          Email = "test@example.com",
          Is_Admin_Flg = false,
          Delete_Flg = false
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act: メソッド呼び出し
        var result = await _controller.GetUserDetails("TestUserId") as OkObjectResult;

        // Assert: 結果の検証
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(200, result.StatusCode, "Status code should be 200.");

        // 明示的な型でキャスト
        var userDetails = result.Value as UserDetailsResponse;
        Assert.IsNotNull(userDetails, "User details should not be null.");
        Assert.AreEqual("TestUser", userDetails.UserName, "UserName should match.");
        Assert.AreEqual("test@example.com", userDetails.Email, "Email should match.");
        Assert.AreEqual(false, userDetails.IsAdminFlg, "IsAdminFlg should match.");

        transaction.Rollback(); // トランザクションをロールバック
      }
    }

    /// <summary>
    /// 指定されたユーザーIDでユーザーを削除するテスト
    /// 成功時に 200 OK と成功メッセージが返ることを検証
    /// </summary>
    [TestMethod]
    public async Task DeleteUser_ReturnsOkResult_WhenUserExists()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        var user = new Users
        {
          Id = "TestUserId",
          UserName = "TestUser",
          Email = "test@example.com",
          Is_Admin_Flg = false,
          Delete_Flg = false
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteUser("TestUserId") as OkObjectResult;

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(200, result.StatusCode, "Status code should be 200.");

        // 型キャストを使用, responseMessage.Message
        var response = result.Value as SuccessResponse_User;
        Assert.IsNotNull(response, "Response should not be null.");
        Assert.AreEqual("ユーザーを削除しました。", response.Message); // 'Message' を使用
        Assert.IsTrue(user.Delete_Flg, "Delete_Flg should be set to true.");
        transaction.Rollback(); // トランザクションをロールバック
      }
    }

    /// <summary>
    /// ユーザー情報を更新するテスト
    /// 成功時に 200 OK と更新された情報が反映されることを検証
    /// </summary>
    [TestMethod]
    public async Task UpdateUser_ReturnsOkResult_WhenUserIsUpdated()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        var user = new Users
        {
          Id = "TestUserId",
          UserName = "OldUser",
          Email = "old@example.com",
          Is_Admin_Flg = false,
          Delete_Flg = false
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdateUserRequest
        {
          UserName = "UpdatedUser",
          Email = "updated@example.com",
          IsAdminFlg = true
        };

        // Act
        var result = await _controller.UpdateUser("TestUserId", updateRequest) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(200, result.StatusCode, "Status code should be 200.");
        var updatedUser = _context.Users.FirstOrDefault(u => u.Id == "TestUserId");
        Assert.IsNotNull(updatedUser, "Updated user should not be null.");
        Assert.AreEqual("UpdatedUser", updatedUser.UserName, "UserName should be updated.");
        Assert.AreEqual("updated@example.com", updatedUser.Email, "Email should be updated.");
        Assert.IsTrue(updatedUser.Is_Admin_Flg, "Is_Admin_Flg should be true.");

        transaction.Rollback(); // トランザクションをロールバック
      }
    }

    /// <summary>
    /// 新しいユーザーを登録するテスト
    /// 成功時に 200 OK とユーザー情報が正しく保存されることを検証
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsOkResult_WhenUserIsRegistered()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        // Arrange
        var registerModel = new RegisterModel
        {
          UserName = "NewUser",
          Email = "newuser@example.com",
          Password = "P@ssw0rd",
          IsAdmin = false
        };

        // Act
        var result = await _controller.Register(registerModel) as OkObjectResult;

        // Assert
        var user = _context.Users.FirstOrDefault(u => u.UserName == "NewUser");
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(200, result.StatusCode, "Status code should be 200.");
        Assert.IsNotNull(user, "Newly registered user should not be null.");
        Assert.AreEqual("NewUser", user.UserName, "UserName should match.");
        Assert.AreEqual("newuser@example.com", user.Email, "Email should match.");

        transaction.Rollback(); // トランザクションをロールバック
      }
    }

    /// <summary>
    /// 登録処理が失敗した場合のテスト
    /// エラーが発生し、400 BadRequest が返されることを検証
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
    {
      using (var transaction = _context.Database.BeginTransaction())
      {
        var registerModel = new RegisterModel
        {
          UserName = "User",
          Email = "user@example.com",
          Password = "aaaaa",//誤ったパスワード
          IsAdmin = false
        };

        // Act: メソッド呼び出し
        var result = await _controller.Register(registerModel) as BadRequestObjectResult;

        // Assert: 結果を検証
        Assert.IsNotNull(result, "Result should not be null.");
        Assert.AreEqual(400, result.StatusCode, "Status code should be 400.");

        // レスポンスの内容を確認
        Assert.IsNotNull(result.Value, "Response should not be null.");
        var response = result.Value as ErrorResponse_User;
        Assert.AreEqual("Registration failed.", (string)response.Message, "Message should match.");
        var errors = (string)response.Errors;
        Assert.IsTrue(errors.Contains("Passwords must have at least one digit ('0'-'9')"), "Duplicate user error should be present.");

        transaction.Rollback(); // トランザクションをロールバック
      }
    }
  }
}
