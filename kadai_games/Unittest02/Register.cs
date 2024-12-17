using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kadai_games.Controllers;
using kadai_games.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

/*namespace AccountControllerTests
{
  [TestClass]
  public class RegisterControllerTests
  {
    private Mock<UserManager<Users>> _registerUserManagerMock;
    private AccountController _registerController; 

    [TestInitialize]
    public void Setup()
    {
      // UserManagerのモックをセットアップ
      _registerUserManagerMock = new Mock<UserManager<Users>>(
          new Mock<IUserStore<Users>>().Object,
          null, null, null, null, null, null, null, null
      );

      // テスト対象のコントローラーを初期化
      _registerController = new AccountController(null, _registerUserManagerMock.Object);
    }

    // --- Register メソッドのテスト ---

    /// <summary>
    /// 登録が成功した場合、200ステータスと成功メッセージを返すか確認
    /// </summary>
    [TestMethod]
    public async Task Register_Should_Return_Ok_If_Registration_Is_Successful()
    {
      // Arrange: テストデータとモックのセットアップ
      var registerRequest = new AccountController.RegisterModel
      {
        Email = "test@example.com",
        Password = "Password123!",
        IsAdmin = true
      };

      // ユーザー作成成功時の動作をモック
      _registerUserManagerMock.Setup(um => um.CreateAsync(It.IsAny<Users>(), registerRequest.Password))
          .ReturnsAsync(IdentityResult.Success);

      // Act
      var result = await _registerController.Register(registerRequest);

      // Assert
      var okResult = result as OkObjectResult;
      Assert.IsNotNull(okResult);

      Assert.AreEqual(200, okResult.StatusCode);
      var response = okResult.Value as dynamic; 
      Assert.IsNotNull(response);
      Assert.AreEqual("User registered successfully.", response.Message); 
    }

    /// <summary>
    /// Emailが空の場合、400ステータスとエラーメッセージを返すか確認
    /// </summary>
    [TestMethod]
    public async Task Register_Should_Return_BadRequest_If_ModelState_Is_Invalid()
    {
      // Arrange: モデルが無効な場合のテストデータをセットアップ
      var registerRequest = new AccountController.RegisterModel
      {
        Email = "", 
        Password = "ValidPassword123!",
        IsAdmin = true
      };

      // ModelStateにエラーを追加
      _registerController.ModelState.AddModelError("Email", "Email is required");

      // Act
      var result = await _registerController.Register(registerRequest);

      // Assert
      var badRequestResult = result as BadRequestObjectResult;
      Assert.IsNotNull(badRequestResult); 
      Assert.AreEqual(400, badRequestResult.StatusCode); 
      Assert.AreEqual("Invalid registration request.", badRequestResult.Value); 
    }

    /// <summary>
    /// ユーザー作成が失敗した場合、400ステータスとエラーメッセージを返すか確認
    /// </summary>
    [TestMethod]
    public async Task Register_Should_Return_BadRequest_If_UserManager_Create_Fails()
    {
      // Arrange: ユーザー作成失敗時のテストデータをセットアップ
      var registerRequest = new AccountController.RegisterModel
      {
        Email = "test@example.com",
        Password = "InvalidPassword", // 無効なパスワード
        IsAdmin = false
      };

      // ユーザー作成が失敗する場合の動作をモック
      _registerUserManagerMock.Setup(um => um.CreateAsync(It.IsAny<Users>(), registerRequest.Password))
          .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

      // Act
      var result = await _registerController.Register(registerRequest);

      // Assert
      var badRequestResult = result as BadRequestObjectResult;
      Assert.IsNotNull(badRequestResult); 
      Assert.AreEqual(400, badRequestResult.StatusCode);

      var errors = badRequestResult.Value as IEnumerable<IdentityError>; 
      Assert.IsNotNull(errors);
      Assert.AreEqual("Password too weak", errors.First().Description); 
    }
  }
}
   */
