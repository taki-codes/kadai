
using kadai_games.Data;
using kadai_games.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// ログ設定の追加
builder.Logging.ClearProviders(); // 既存のログプロバイダーをクリア
builder.Logging.AddConsole();    // コンソールにログを出力

// CORSポリシーの設定
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowSpecificOrigin", policy =>
  {
    policy.WithOrigins("https://localhost:51053") // フロントエンドの正しいURL
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials(); // withCredentials をサポート
  });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext の登録
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identityの設定（認証・ユーザ管理）
builder.Services.AddIdentity<Users, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cookie認証の設定
builder.Services.ConfigureApplicationCookie(options =>
{
  options.Cookie.HttpOnly = true;
  options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
  options.LoginPath = "/api/account/login";
  options.LogoutPath = "/api/account/logout";
  options.AccessDeniedPath = "/api/account/access-denied";

});

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseCors("AllowSpecificOrigin"); // CORSを適用
// キャッシュ無効化ミドルウェアを追加
app.Use(async (context, next) =>
{
  context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
  context.Response.Headers["Pragma"] = "no-cache";
  context.Response.Headers["Expires"] = "0";
  await next();
});
app.UseRouting();
app.UseAuthentication();
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");


app.Run();
