using System.ComponentModel.DataAnnotations;

public class Genre
{
  [Key]
  [Required]
  public int Genre_Id { get; set; }
  [MaxLength(100)]
  [Required]
  public string Genre_Name { get; set; } = string.Empty;
  [MaxLength(50)]
  [Required]
  public string CreatedUser { get; set; } = string.Empty;
  public DateTime CreateDate { get; set; }
  [Required]
  public bool Delete_Flg { get; set; } = false;
}
