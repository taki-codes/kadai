using Microsoft.AspNetCore.Identity;

namespace kadai_games.Models
{
  public class Users : IdentityUser
  {
    public bool Is_Admin_Flg { get; set; } // 管理者フラグ
  } 
}
