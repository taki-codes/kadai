using System.Threading.Tasks;
using System.Security.Claims;
using kadai_games.Controllers;
using kadai_games.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Http;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using static kadai_games.Controllers.AccountController;

namespace AccountControllerTests
{
  [TestClass]
  public class AccountControllerTests
  {
    private Mock<UserManager<Users>> _userManagerMock;
    private Mock<SignInManager<Users>> _signInManagerMock;
    private AccountController _controller;

    [TestInitialize]
    public void Setup()
    {
      // UserManager のモックをセットアップ
      _userManagerMock = new Mock<UserManager<Users>>(
          new Mock<IUserStore<Users>>().Object,
          null, null, null, null, null, null, null, null
      );

      // SignInManager のモックをセットアップ
      _signInManagerMock = new Mock<SignInManager<Users>>(
          _userManagerMock.Object,
          new Mock<IHttpContextAccessor>().Object,
          new Mock<IUserClaimsPrincipalFactory<Users>>().Object,
          null, null, null, null
      );

      _controller = new AccountController(_signInManagerMock.Object, _userManagerMock.Object);
    }

    /// <summary>
    /// 共通アサートメソッド：Unauthorizedレスポンスを検証
    /// </summary>
    private void AssertUnauthorizedResult(IActionResult result, string expectedMessage)
    {
      var unauthorizedResult = result as UnauthorizedObjectResult;
      Assert.IsNotNull(unauthorizedResult); 
      Assert.AreEqual(401, unauthorizedResult.StatusCode); 
      Assert.AreEqual(expectedMessage, unauthorizedResult.Value); 
    }

    // --- Login メソッドのテスト ---

    /// <summary>
    /// 有効のユーザーの場合にログインが成功するか確認
    /// </summary>
    [TestMethod]
    public async Task Login_Should_Return_Ok_If_Credentials_Are_Valid()
    {
      // Arrange: テスト用のユーザーとリクエストデータをセットアップ
      var user = new Users { Email = "test@example.com", Is_Admin_Flg = true };
      var loginRequest = new AccountController.GetAccount
      {
        Email = "test@example.com",
        Password = "Password123!"
      };

      _userManagerMock.Setup(um => um.FindByEmailAsync(loginRequest.Email))
          .ReturnsAsync(user);

      _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(user, loginRequest.Password, false, false))
          .ReturnsAsync(SignInResult.Success);

      // Act
      var result = await _controller.Login(loginRequest);

      // Assert
      var okResult = result as OkObjectResult;
      Assert.IsNotNull(okResult); 
      Assert.AreEqual(200, okResult.StatusCode); 

      var response = okResult.Value as dynamic;
      Assert.AreEqual(true, response.isAdmin); 
    }

    /// <summary>
    /// 無効なユーザーの場合に Unauthorized が返されるか確認
    /// </summary>
    [TestMethod]
    public async Task Login_Should_Return_Unauthorized_If_Credentials_Are_Invalid()
    {
      // Arrange: 無効なリクエストデータをセットアップ
      var loginRequest = new AccountController.GetAccount
      {
        Email = "invalid@example.com",
        Password = "WrongPassword"
      };

      _userManagerMock.Setup(um => um.FindByEmailAsync(loginRequest.Email))
          .ReturnsAsync((Users)null); 

      // Act
      var result = await _controller.Login(loginRequest);

      // Assert
      AssertUnauthorizedResult(result, "Invalid credentials");
    }

    /// <summary>
    /// メールアドレスが空の場合に BadRequest が返されるか確認
    /// </summary>
    [TestMethod]
    public async Task Login_Should_Return_BadRequest_If_Email_Or_Password_Is_Empty()
    {
      // Arrange: 無効なリクエストデータと ModelState をセットアップ
      var loginRequest = new AccountController.GetAccount
      {
        Email = "",
        Password = "Password123!"
      };

      // ModelStateを無効化
      _controller.ModelState.AddModelError("Email", "Email is required");

      // Act: 
      var result = await _controller.Login(loginRequest);

      // Assert: 
      var badRequestResult = result as BadRequestObjectResult;
      Assert.IsNotNull(badRequestResult); 
      Assert.AreEqual(400, badRequestResult.StatusCode); 
      Assert.AreEqual("Invalid login request", badRequestResult.Value); 
    }

    /// <summary>
    /// アカウントがロックされている場合に Unauthorized が返されるか確認
    /// </summary>
    [TestMethod]
    public async Task Login_Should_Return_Unauthorized_If_User_Is_LockedOut()
    {
      // Arrange: ロックアウトされたユーザーをセットアップ
      var user = new Users { Email = "test@example.com", Is_Admin_Flg = false };
      var loginRequest = new AccountController.GetAccount
      {
        Email = "test@example.com",
        Password = "Password123!"
      };

      _userManagerMock.Setup(um => um.FindByEmailAsync(loginRequest.Email))
          .ReturnsAsync(user);

      _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(user, loginRequest.Password, false, false))
          .ReturnsAsync(SignInResult.LockedOut);

      // Act
      var result = await _controller.Login(loginRequest);

      // Assert
      AssertUnauthorizedResult(result, "Invalid credentials");
    }

    // --- GetUserInfo メソッドのテスト ---

    /// <summary>
    /// 認証されたユーザー情報が正しく返されるか確認
    /// </summary>
   [TestMethod]
public async Task GetUserInfo_Should_Return_UserInfo_If_Authorized()
{
    // Arrange: テスト用のユーザーをセットアップ
    var user = new Users { Email = "test@example.com", Is_Admin_Flg = true };

    _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
        .ReturnsAsync(user);

    // Act: GetUserInfo メソッドを実行
    var result = await _controller.GetUserInfo();

    // Assert: 成功レスポンスを検証
    var okResult = result as OkObjectResult;
    Assert.IsNotNull(okResult); 
    Assert.AreEqual(200, okResult.StatusCode);

    var response = okResult.Value as UserInfoResponse; 
    Assert.IsNotNull(response); 
    Assert.AreEqual(true, response.IsAdmin); 
}
    /// <summary>
    /// ユーザーが見つからない場合に Unauthorized が返されるか確認
    /// </summary>
    [TestMethod]
    public async Task GetUserInfo_Should_Return_Unauthorized_If_User_Not_Found()
    {
      // Arrange: ユーザーが存在しないケースをセットアップ
      _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
          .ReturnsAsync((Users)null);

      // Act
      var result = await _controller.GetUserInfo();

      // Assert
      AssertUnauthorizedResult(result, "User not found");
    }
  }
}
