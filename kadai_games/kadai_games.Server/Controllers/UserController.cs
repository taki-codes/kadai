using kadai_games.Data;
using kadai_games.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace kadai_games.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Users> _userManager;

    public UserController(ApplicationDbContext context, UserManager<Users> userManager)
    {

      _userManager = userManager;
      _context = context;
    }

    [HttpGet("users")]
    public IActionResult GetUsers([FromQuery] string? searchText, [FromQuery] bool? isAdmin)
    {
      // クエリベースでユーザをフィルタリング
      var query = _context.Users
          .Where(g => !g.Delete_Flg); // 削除フラグが立っていないユーザーを対象

      // あいまい検索（UserName または Email が検索条件に一致する場合）
      if (!string.IsNullOrEmpty(searchText))
      {
        query = query.Where(u =>
            u.UserName.Contains(searchText) || // UserNameに含まれる
            u.Email.Contains(searchText));    // Emailに含まれる
      }

      // 管理者フラグの条件を追加
      if (isAdmin.HasValue)
      {
        query = query.Where(u => u.Is_Admin_Flg == isAdmin.Value);
      }

      // 結果を選択して返す
      var users = query
          .Select(u => new UserResponse
          {
            Id = u.Id, // ID を追加
            UserName = u.UserName,
            Email = u.Email,
            IsAdminFlg = u.Is_Admin_Flg
          })
          .ToList();

      return Ok(users);
    }


    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserDetails(string id)
    {
      // ユーザーをIDで検索
      var user = await _context.Users
          .Where(u => !u.Delete_Flg && u.Id == id) // 削除フラグが立っていない
          .Select(u => new UserDetailsResponse
          {
            UserName = u.UserName,
            Email = u.Email,
            IsAdminFlg = u.Is_Admin_Flg
          })
          .FirstOrDefaultAsync();

      if (user == null)
      {
        return NotFound(new { message = "ユーザーが見つかりませんでした。" });
      }

      return Ok(user);
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
      // ユーザーをIDで検索
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.Delete_Flg);
   
      // 削除フラグを立てる
      user.Delete_Flg = true;

      _context.Users.Update(user);
      await _context.SaveChangesAsync();

      return Ok(new SuccessResponse_User { Message = "ユーザーを削除しました。" });
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
    {
      // ユーザーをIDで検索
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.Delete_Flg);
     
      // フィールドを更新
      user.UserName = request.UserName ?? user.UserName; // nullの場合は現状維持
      user.Email = request.Email ?? user.Email;
      user.Is_Admin_Flg = request.IsAdminFlg ?? user.Is_Admin_Flg;

      _context.Users.Update(user);
      await _context.SaveChangesAsync();

      return Ok(new { message = "ユーザー情報を更新しました。" });
    }

    [HttpPost("register")]
      public async Task<IActionResult> Register([FromBody] RegisterModel model)
      {
       
      // 新しいユーザーを作成
      var user = new Users
      {
        UserName = model.UserName,
        Email = model.Email,
        Is_Admin_Flg = model.IsAdmin
      };

      // ユーザー登録処理
      var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
      {
        return BadRequest(new ErrorResponse_User
        {
          Message = "Registration failed.",
          Errors = string.Join(" ", result.Errors.Select(e => e.Description)) // エラーメッセージを結合
        });
      }

      // 登録成功メッセージを返却
      return Ok(new
      {
        Message = "User registered successfully.",
        UserId = user.Id
      });
    }
    public class SuccessResponse_User
    {
      public string Message { get; set; } // プロパティ名が 'Message' であることを確認
    }

    public class UserDetailsResponse
    {
      public string UserName { get; set; }
      public string Email { get; set; }
      public bool IsAdminFlg { get; set; }
    }

    public class UserResponse
    {
      public string Id { get; set; }
      public string UserName { get; set; }
      public string Email { get; set; }
      public bool IsAdminFlg { get; set; }
    }

    public class ErrorResponse_User
    {
      public string Message { get; set; }
      public string Errors { get; set; }
    }
  
    // 登録リクエストモデル
    public class RegisterModel
    {
      public string UserName { get; set; } // ユーザー名
      public string Email { get; set; }      // メールアドレス
      public string Password { get; set; }  // パスワード
      public bool IsAdmin { get; set; }     // 管理者フラグ
    }
    // リクエストモデル
    public class UpdateUserRequest
    {
      public string? UserName { get; set; }
      public string? Email { get; set; }
      public bool? IsAdminFlg { get; set; }
    }

  }
}
