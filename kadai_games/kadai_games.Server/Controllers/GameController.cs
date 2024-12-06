using kadai_games.Data;
using kadai_games.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace kadai_games.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class GameController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public GameController(ApplicationDbContext context)
    {
      _context = context;
    }

    // ゲーム一覧を取得
    [HttpGet]
    public IActionResult GetGames(string title = "", string maker = "", string genre = "")
    {
      var query = _context.Games
          .Where(g => !g.Delete_Flg)
          .Select(g => new
          {
            g.Game_Id,
            g.Title,
            g.Maker_Id,
            Maker_Name = g.Maker.Maker_Name,
            g.Genre_Id,
            Genre_Name = g.Genre.Genre_Name,
            g.Sales_Count,
            g.Memo,
            g.CreateDate,
            g.CreatedUser,
            g.UpdateDate,
            g.UpdateUser
          })
          .AsQueryable();

      // フィルタリング
      if (!string.IsNullOrEmpty(title))
        query = query.Where(g => g.Title.Contains(title));

      if (!string.IsNullOrEmpty(maker))
        query = query.Where(g => g.Maker_Name.Contains(maker));

      if (!string.IsNullOrEmpty(genre))
        query = query.Where(g => g.Genre_Name.Contains(genre));

      return Ok(query.ToList());
    }

    // ゲーム新規作成
    [HttpPost]
    public IActionResult CreateGame([FromBody] GameViewModel model)
    {
      if (!ModelState.IsValid)
      {
        // バリデーションエラーメッセージを返す
        var errors = ModelState
       .Where(kv => kv.Value.Errors.Any()) // エラーがある項目だけ取得
       .ToDictionary(
           kv => kv.Key, // フィールド名
           kv => kv.Value.Errors.Select(e => e.ErrorMessage).ToArray() // エラーメッセージ配列
       );

        return BadRequest(new { errors });
      }

      var genre = _context.Genres.Find(model.Genre_Id);
      var maker = _context.Makers.Find(model.Maker_Id);

      if (genre == null || maker == null)
      {
        return BadRequest(new
        {
          message = "無効な Genre_Id または Maker_Id です。",
          details = new { Genre_Id = model.Genre_Id, Maker_Id = model.Maker_Id }
        });
      }

      var game = new Game
      {
        Title = model.Title,
        Maker_Id = maker.Maker_Id,
        Genre_Id = genre.Genre_Id,
        Sales_Count = model.Sales_Count,
        Memo = model.Memo,
        CreateDate = DateTime.Now,
        UpdateDate = DateTime.Now,
        CreatedUser = "admin",
        UpdateUser = "admin",
        Delete_Flg = false
      };


      _context.Games.Add(game);
      _context.SaveChanges();

      return Ok(game);
    }
    // ジャンル一覧を取得
    [HttpGet("genres")]
    public IActionResult GetGenres()
    {
      var genres = _context.Genres.Select(g => new
      {
        g.Genre_Id,
        g.Genre_Name
      }).ToList();

      return Ok(genres);
    }

    // メーカー一覧を取得
    [HttpGet("makers")]
    public IActionResult GetMakers()
    {
      try
      {
        var makers = _context.Makers.Select(m => new
        {
          m.Maker_Id,
          m.Maker_Name,
          m.Maker_Address
        }).ToList();

        return Ok(makers);
      }
      catch (Exception ex)
      {
        return BadRequest(new
        {
          message = "メーカー情報の取得に失敗しました。",
          error = ex.Message
        });
      }
    }

    // ゲーム詳細を取得
    [HttpGet("{id}")]
    public IActionResult GetGame(int id)
    {
      var game = _context.Games.FirstOrDefault(g => g.Game_Id == id && !g.Delete_Flg);
      if (game == null)
        return NotFound(new { message = "指定されたゲームが見つかりません。" });

      return Ok(game);
    }

    // ゲーム情報を更新
      [HttpPut("{id}")]
      public IActionResult UpdateGame(int id, [FromBody] Game updatedGame)
      {
        var game = _context.Games.FirstOrDefault(g => g.Game_Id == id && !g.Delete_Flg);
        if (game == null)
          return NotFound(new { message = "指定されたゲームが見つかりません。" });

        game.Title = updatedGame.Title;
        game.Maker_Id = updatedGame.Maker_Id;
        game.Genre_Id = updatedGame.Genre_Id;
        game.Sales_Count = updatedGame.Sales_Count;
        game.Memo = updatedGame.Memo;
        game.UpdateDate = DateTime.Now;
        game.UpdateUser = "admin";

        _context.SaveChanges();

        return Ok(game);
      }

     // 削除 (論理削除)
    [HttpDelete("{id}")]
    public IActionResult DeleteGame(int id)
    {
      var game = _context.Games.FirstOrDefault(g => g.Game_Id == id && !g.Delete_Flg);
      if (game == null)
        return NotFound(new { message = "指定されたゲームが見つかりません。" });

      game.Delete_Flg = true;
      game.UpdateDate = DateTime.Now;
      game.UpdateUser = "admin";

      _context.SaveChanges();

      return Ok(new { message = "ゲームを削除しました。" });
    }
  }
}
