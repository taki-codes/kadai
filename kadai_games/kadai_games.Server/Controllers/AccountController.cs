using kadai_games.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace kadai_games.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AccountController : ControllerBase
  {
    private readonly SignInManager<Users> _signInManager;
    private readonly UserManager<Users> _userManager;

    public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager)
    {
      _signInManager = signInManager;
      _userManager = userManager;
    }

    // ログイン処理
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] GetAccount loginRequest)
    {
      if (!ModelState.IsValid)
        return BadRequest("Invalid login request");

      var user = await _userManager.FindByEmailAsync(loginRequest.Email);
      if (user == null)
        return Unauthorized("Invalid credentials");

      var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, false, false);
      if (!result.Succeeded)
        return Unauthorized("Invalid credentials");

      return Ok(new LoginResponse
      {
        message = "Login successful",
        isAdmin = user.Is_Admin_Flg // ユーザーの管理者フラグを返す
      });
    }

    // ログインユーザ情報取得
    [Authorize]
    [HttpGet("userinfo")]
    public async Task<IActionResult> GetUserInfo()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null)
      {
        return Unauthorized("User not found");
      }

      return Ok(new UserInfoResponse { IsAdmin = user.Is_Admin_Flg });
    }

    // ユーザのログアウト処理

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
      await _signInManager.SignOutAsync();
      Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
      Response.Headers["Pragma"] = "no-cache";
      Response.Headers["Expires"] = "0";

      return Ok(new LogoutResponse
      {
        Message = "Logged out successfully"
      });
    }

    // 新規ユーザ登録処理
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
      // 入力データの検証
      if (!ModelState.IsValid)
        return BadRequest("Invalid registration request.");

      // 新しいユーザーを作成
      var user = new Users
      {
        UserName = model.Email,   // ユーザー名に Email を使用
        Email = model.Email,      // メールアドレスを登録
        Is_Admin_Flg = model.IsAdmin // 管理者フラグを登録
      };

      // パスワードを含めてユーザーを作成
      var result = await _userManager.CreateAsync(user, model.Password);

      if (!result.Succeeded)
        return BadRequest(result.Errors);

      // 登録成功メッセージとユーザIDを返却
      return Ok(new 
      {
        Message = "User registered successfully.",
        UserId = user.Id
      });
    }


    // 登録リクエストモデル
    public class RegisterModel
    {
      public string Email { get; set; }      // メールアドレス
      public string Password { get; set; }  // パスワード
      public bool IsAdmin { get; set; }     // 管理者フラグ
    }
    // ログインユーザーモデル
    public class GetAccount
    {
      public string Email { get; set; }
      public string Password { get; set; }
    }
  }
  public class LoginResponse
  {
    public string message { get; set; }
    public bool isAdmin { get; set; }
  }
  public class UserInfoResponse
  {
    public bool IsAdmin { get; set; }
  }

  public class LogoutResponse
  {
    public string Message { get; set; }
  }

}
