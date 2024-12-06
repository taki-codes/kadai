using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Game
{
  [Key]
  [Required]
  public int Game_Id { get; set; }
  [MaxLength(200)]
  [Required]
  public string Title { get; set; } = string.Empty;
  [ForeignKey("Maker")]
  [Required]
  public int Maker_Id { get; set; }
  [JsonIgnore] // JSON に含めない
  [ValidateNever]
  public Maker Maker { get; set; }
  [ForeignKey("Genre")]
  [Required]
  public int Genre_Id { get; set; }
  [JsonIgnore] // JSON に含めない
  [ValidateNever]
  public Genre Genre { get; set; }
  [Required]
  public int Sales_Count { get; set; }
  public string? Memo { get; set; }
  [MaxLength(50)]
  [Required]
  public string CreatedUser { get; set; } = string.Empty;
  public DateTime CreateDate { get; set; }
  [MaxLength(50)]
  [Required]
  public string UpdateUser { get; set; } = string.Empty;
  public DateTime UpdateDate { get; set; }
  [Required]
  public bool Delete_Flg { get; set; } = false;
}
