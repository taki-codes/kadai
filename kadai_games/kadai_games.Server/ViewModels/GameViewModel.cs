using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace kadai_games.Server.ViewModels
{
  public class GameViewModel
  {
   
    [MaxLength(200)]
    [Required(ErrorMessage = "タイトル入力してください")]
    public string? Title { get; set; } = string.Empty;
   
    [Required(ErrorMessage = "メーカ名を選んでください")]
    public int? Maker_Id { get; set; }
    
    [Required(ErrorMessage = "ジャンル名を選んでください")]
    public int? Genre_Id { get; set; }

    [Required(ErrorMessage = "売り上げ件数を入力してください")]
    public int Sales_Count { get; set; }
    public string? Memo { get; set; }
    

   
  }
}
