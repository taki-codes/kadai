using System.Threading.Tasks;
using kadai_games.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Http;
using kadai_games.Models;

namespace AccountControllerTests
{
  [TestClass]
  public class LogoutControllerTests
  {
    private Mock<SignInManager<Users>> _logoutSignInManagerMock;
    private AccountController _logoutController;

    [TestInitialize]
    public void Setup()
    {
      // SignInManagerのモックを作成
      _logoutSignInManagerMock = new Mock<SignInManager<Users>>(
          new Mock<UserManager<Users>>(new Mock<IUserStore<Users>>().Object, null, null, null, null, null, null, null, null).Object,
          new Mock<IHttpContextAccessor>().Object,
          new Mock<IUserClaimsPrincipalFactory<Users>>().Object,
          null, null, null, null
      );

      // テスト対象のコントローラーを初期化
      _logoutController = new AccountController(_logoutSignInManagerMock.Object, null);
    }

    /// <summary>
    /// ログアウト成功時に200ステータスが返されるか確認
    /// </summary>
    [TestMethod]
    public async Task Logout_Should_Return_Ok_When_Logout_Is_Successful()
    {
      // Arrange: SignOutAsyncの動作をモック
      _logoutSignInManagerMock.Setup(sm => sm.SignOutAsync())
          .Returns(Task.CompletedTask);

      // HttpContextとレスポンスのモックを設定
      var httpContextMock = new Mock<HttpContext>();
      var responseMock = new Mock<HttpResponse>();
      var headers = new HeaderDictionary();

      responseMock.SetupGet(r => r.Headers).Returns(headers);
      httpContextMock.SetupGet(h => h.Response).Returns(responseMock.Object);

      _logoutController.ControllerContext = new ControllerContext
      {
        HttpContext = httpContextMock.Object
      };

      // Act
      var result = await _logoutController.Logout();

      // Assert
      var okResult = result as OkObjectResult;
      Assert.IsNotNull(okResult); 
      Assert.AreEqual(200, okResult.StatusCode); 
      var response = okResult.Value as LogoutResponse;
      Assert.IsNotNull(response); 
      Assert.AreEqual("Logged out successfully", response.Message); 
    }

    /// <summary>
    /// ログアウト時に正しいレスポンスヘッダー(キャッシュ無効)が設定されるか確認
    /// </summary>
    [TestMethod]
    public async Task Logout_Should_Set_Response_Headers()
    {
      // Arrange: SignOutAsyncの動作をモック
      _logoutSignInManagerMock.Setup(sm => sm.SignOutAsync())
          .Returns(Task.CompletedTask);

      // HttpContextとレスポンスのモックを設定
      var httpContextMock = new Mock<HttpContext>();
      var responseMock = new Mock<HttpResponse>();
      var headers = new HeaderDictionary();

      responseMock.SetupGet(r => r.Headers).Returns(headers);
      httpContextMock.SetupGet(h => h.Response).Returns(responseMock.Object);

      _logoutController.ControllerContext = new ControllerContext
      {
        HttpContext = httpContextMock.Object
      };

      // Act
      await _logoutController.Logout();

      // Assert
      Assert.AreEqual("no-cache, no-store, must-revalidate", headers["Cache-Control"]);
      Assert.AreEqual("no-cache", headers["Pragma"]);
      Assert.AreEqual("0", headers["Expires"]);
    }

    /// <summary>
    /// SignOutAsyncが失敗した場合に例外がスローされるか確認
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task Logout_Should_Throw_Exception_If_SignOutAsync_Fails()
    {
      // Arrange: SignOutAsyncが例外をスローするように設定
      _logoutSignInManagerMock.Setup(sm => sm.SignOutAsync())
          .Throws(new Exception("SignOut failed"));

      var httpContextMock = new Mock<HttpContext>();
      httpContextMock.SetupGet(h => h.Response).Returns(new Mock<HttpResponse>().Object);

      _logoutController.ControllerContext = new ControllerContext
      {
        HttpContext = httpContextMock.Object
      };

      // Act
      await _logoutController.Logout();
    }
  }
}
