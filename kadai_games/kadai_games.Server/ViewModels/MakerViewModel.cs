using System.ComponentModel.DataAnnotations;

namespace kadai_games.Server.ViewModels
{
  public class MakerViewModel
  {
    [MaxLength(100, ErrorMessage = "メーカー名は100文字以内で入力してください")]
    [Required(ErrorMessage = "メーカー名を入力してください")]
    public string Maker_Name { get; set; } = string.Empty;
    [MaxLength(200, ErrorMessage = "住所は200文字以内で入力してください")]
    public string? Maker_Address { get; set; }
  }
}
