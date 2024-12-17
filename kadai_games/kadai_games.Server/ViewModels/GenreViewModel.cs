using System.ComponentModel.DataAnnotations;

namespace kadai_games.Server.ViewModels
{
  public class GenreViewModel
  {
    [MaxLength(100)]
    [Required(ErrorMessage = "ジャンル名を入力してください")]
    public string Genre_Name { get; set; } = string.Empty;
  }
}
