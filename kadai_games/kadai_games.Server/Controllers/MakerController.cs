using kadai_games.Data;
using kadai_games.Models;
using kadai_games.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kadai_games.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class MakerController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public MakerController(ApplicationDbContext context)
    {
      _context = context;
    }

    // メーカー一覧を取得
    [HttpGet("makers")]
    public IActionResult GetMakers([FromQuery] string? searchText)
    {
      // 削除フラグが立っていないメーカーのみ対象
      var query = _context.Makers
          .Where(m => !m.Delete_Flg);

      // 削除フラグが立っていないメーカーを取得
      if (!string.IsNullOrEmpty(searchText))
      {
        query = query.Where(m => m.Maker_Name.Contains(searchText) ||
         m.Maker_Address.Contains(searchText)); 
      }

      var makers = query
          .Select(m => new
          {
            m.Maker_Id,
            m.Maker_Name,
            m.Maker_Address
          })
          .ToList();

      return Ok(makers);
    }

    // メーカー新規作成
    [HttpPost]
    public IActionResult CreateMaker([FromBody] MakerViewModel model)
    {
      // 同じ名前のメーカーが既に存在するか確認
      var existingMaker = _context.Makers
          .FirstOrDefault(m => m.Maker_Name == model.Maker_Name && !m.Delete_Flg);

      if (existingMaker != null)
      {
        return BadRequest(new { Message = "同じメーカー名が既に存在します。" });
      }

      var maker = new Maker
      {
        Maker_Name = model.Maker_Name,
        Maker_Address = model.Maker_Address,
        CreateDate = DateTime.Now,
        CreatedUser = "admin",
        Delete_Flg = false
      };

      _context.Makers.Add(maker);
      _context.SaveChanges();

      return Ok(maker);
    }

    // メーカー詳細取得
    [HttpGet("makers/{id}")]
    public IActionResult GetMakerDetails(int id)
    {
      var maker = _context.Makers.FirstOrDefault(m => m.Maker_Id == id && !m.Delete_Flg);

      return Ok(maker);
    }

    // メーカー削除
    [HttpDelete("makers/{id}")]
    public IActionResult DeleteMaker(int id)
    {
      var maker = _context.Makers.FirstOrDefault(m => m.Maker_Id == id && !m.Delete_Flg);

      // メーカーに関連付いているゲーム一覧が存在するか確認
      var relatedGames = _context.Games.Any(g => g.Maker_Id == id && !g.Delete_Flg);
      if (relatedGames)
      {
        return BadRequest(new { Message = "このメーカーはゲーム一覧に存在するため削除できません。" });
      }

      maker.Delete_Flg = true;
      maker.CreateDate = DateTime.Now;
      maker.CreatedUser = "admin";

      _context.SaveChanges();

      return Ok(new { Message = "メーカーを削除しました。" });
    }

    // メーカー更新
    [HttpPut("makers/{id}")]
    public IActionResult UpdateMaker(int id, [FromBody] MakerViewModel updatedMaker)
    {
      var maker = _context.Makers.FirstOrDefault(m => m.Maker_Id == id && !m.Delete_Flg);

      // 同じ名前のメーカーが既に存在するか確認
      var existingMaker = _context.Makers
          .FirstOrDefault(m => m.Maker_Name == updatedMaker.Maker_Name && m.Maker_Id != id && !m.Delete_Flg);

      if (existingMaker != null)
      {
        return BadRequest(new { Message = "同じメーカー名が既に存在します。" });
      }

      maker.Maker_Name = updatedMaker.Maker_Name;
      maker.Maker_Address = updatedMaker.Maker_Address;
      maker.CreateDate = DateTime.Now;
      maker.CreatedUser = "admin";

      _context.SaveChanges();

      return Ok(maker);
    }
  }
}
