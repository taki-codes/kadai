using System.ComponentModel.DataAnnotations;

public class Maker
{
  [Key]
  [Required]
  public int Maker_Id { get; set; }
  [MaxLength(100)]
  [Required]
  public string Maker_Name { get; set; } = string.Empty;
  [MaxLength(200)]
  public string? Maker_Address { get; set; }
  [MaxLength(50)]
  [Required]
  public string CreatedUser { get; set; } = string.Empty;
  public DateTime CreateDate { get; set; }
  [Required]
  public bool Delete_Flg { get; set; } = false;
}
