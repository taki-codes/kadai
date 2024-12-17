using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace kadai_games.Models
{
  public class Users : IdentityUser
  {
    public bool Is_Admin_Flg { get; set; } // 管理者フラグ
    public bool Delete_Flg { get; set; } = false; //削除フラグ
  }

  // ログインユーザーモデル
  public class GetAccount
  {
    public string Email { get; set; }
    public string Password { get; set; }
  }
}
