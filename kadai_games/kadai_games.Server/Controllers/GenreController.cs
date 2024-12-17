using kadai_games.Controllers;
using kadai_games.Data;
using kadai_games.Models;
using kadai_games.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static kadai_games.Controllers.UserController;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace kadai_games.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class GenreController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public GenreController(ApplicationDbContext context)
    {
      _context = context;
    }
    // ジャンル一覧を取得
    [HttpGet("genres")]
    public IActionResult GetGenres([FromQuery] string? searchText)
    {
      // クエリベースでユーザをフィルタリング 
      var query = _context.Genres
          .Where(g => !g.Delete_Flg); // 削除フラグが立っていないユーザーを対象

      // 検索文字列によるフィルタリング
      if (!string.IsNullOrEmpty(searchText))
      {
        query = query.Where(u =>
          u.Genre_Name.Contains(searchText));
      }

      // クエリを実行して結果を取得
      var genres = query
          .Select(g => new
          {
            g.Genre_Id,
            g.Genre_Name
          })
          .ToList();

      return Ok(genres);
    }

    // ジャンル新規作成
    [HttpPost]
    public IActionResult CreateGenre([FromBody] GenreViewModel model)
    {
      // 同じGenre_Nameが存在するか確認
      var existingGenre = _context.Genres
          .FirstOrDefault(g => g.Genre_Name == model.Genre_Name && !g.Delete_Flg);

      if (existingGenre != null)
      {
        return BadRequest(new { Message = "同じジャンル名が既に存在します。" });
      }

      var genre = new Genre
      {
        Genre_Name = model.Genre_Name,
        CreateDate = DateTime.Now,
        CreatedUser = "admin",
        Delete_Flg = false
      };

      _context.Genres.Add(genre);
      _context.SaveChanges();

      return Ok(genre); 
      
    }

    // ジャンル詳細取得
    [HttpGet("genres/{id}")]
    public IActionResult GetGenreDetails(int id)
    {
      var genre = _context.Genres.FirstOrDefault(g => g.Genre_Id == id && !g.Delete_Flg);
   
      return Ok(genre);
    }

    // ジャンル削除
    [HttpDelete("genres/{id}")]
    public IActionResult DeleteGenre(int id)
    {
      var genre = _context.Genres.FirstOrDefault(g => g.Genre_Id == id && !g.Delete_Flg);

      // ジャンルに関連付いているゲーム一覧が存在するか確認
      var relatedGames = _context.Games.Any(g => g.Genre_Id == id && !g.Delete_Flg);
      if (relatedGames)
      {
        return BadRequest(new { Message = "このジャンルはゲーム一覧に存在するため削除できません。" });
      }

      genre.Delete_Flg = true;
      genre.CreateDate = DateTime.Now;
      genre.CreatedUser = "admin";

      _context.SaveChanges();

      return Ok(new { Message = "ジャンルを削除しました。" });
    }

    // ジャンル更新
    [HttpPut("genres/{id}")]
    public IActionResult UpdateGenre(int id, [FromBody] Genre updatedGenre)
    {
      var genre = _context.Genres.FirstOrDefault(g => g.Genre_Id == id && !g.Delete_Flg);

      // 他のレコードに同じGenre_Nameが存在するか確認
      var existingGenre = _context.Genres
          .FirstOrDefault(g => g.Genre_Name == updatedGenre.Genre_Name && g.Genre_Id != id && !g.Delete_Flg);

      if (existingGenre != null)
      {
        return BadRequest(new { Message = "同じジャンル名が既に存在します。" });
      }

      genre.Genre_Name = updatedGenre.Genre_Name;
      genre.CreateDate = DateTime.Now;
      genre.CreatedUser = "admin";

      _context.SaveChanges();

      return Ok(genre);
    }
  }

}


