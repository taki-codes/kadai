# Kadai Games — ゲーム情報管理アプリ

ゲームのタイトル・メーカー・ジャンル・販売数を管理する、Angular と ASP.NET Core の Web アプリです。ゲーム一覧から関連マスターの管理までを扱い、画面、API、データベースを分けた構成で CRUD とログイン処理を実装しています。

学習用プロジェクトです。現在の認証は **ASP.NET Core Identity の Cookie 認証**です。

## 主な機能

- メールアドレスとパスワードによるログイン・ログアウト、ログイン状態に応じた画面遷移
- ゲームの一覧・詳細表示、新規登録・更新・論理削除
- タイトル・メーカー・ジャンルでの検索、一覧の並び替え、検索条件の URL への保持
- メーカー・ジャンル・ユーザーのマスター管理
- 管理者フラグを用いたメニュー表示の切り替え
- マスター名の重複確認、ゲームから参照中のマスターの削除制限

## 技術構成

| 領域 | 使用技術 |
| --- | --- |
| フロントエンド | Angular 18、TypeScript、RxJS、Bootstrap 5、Angular Material / CDK 17 |
| バックエンド | C#、ASP.NET Core 8 Web API、ASP.NET Core Identity |
| データベース | SQL Server、Entity Framework Core 8、EF マイグレーション |
| API ドキュメント | Swagger / OpenAPI |
| テスト | MSTest、Moq、Jasmine / Karma |

```text
Angular の画面・サービス
        │ HTTP
        ▼
ASP.NET Core の Controller
        │ Entity Framework Core / Identity
        ▼
SQL Server
```

## コードの見どころ

- **関連を持つデータの CRUD**: ゲームとメーカー・ジャンルを外部キーで関連付け、一覧では名前を取得して表示します。
- **論理削除と参照チェック**: `Delete_Flg` を用いて一覧対象を制御し、関連ゲームが残るマスターの削除を防ぎます。
- **画面と API の分離**: Angular のサービス層が API 通信を担当し、Controller、モデル、ViewModel をサーバー側に配置しています。
- **テストによる振る舞いの確認**: ログイン、ゲーム、ユーザー、メーカー、ジャンルを対象に、正常系や入力エラーを確認するテストコードを収録しています。

## ディレクトリ

```text
kadai_games/
├── kadai_games.sln
├── kadai_games.Server/       # Web API、Identity、DB モデル、マイグレーション
├── kadai_games.client/       # Angular の画面・ルーティング・API サービス
├── Unittest02/              # アカウント関連テスト
├── Unitttest_Games/          # ゲーム関連テスト
├── Unittest_Masters_Genre/   # ジャンル関連テスト
├── Unittest_Masters_Maker/   # メーカー関連テスト
└── Unittest_Masters_User/    # ユーザー関連テスト
```

## ローカル開発

### 必要な環境

- .NET SDK 8
- Angular 18 と互換性のある Node.js と npm
- SQL Server。既存テストは Windows の SQL Server LocalDB を前提としています。
- Visual Studio 2022 で開く場合は、ASP.NET と Web 開発、JavaScript / TypeScript の開発環境

### セットアップ

```powershell
git clone https://github.com/taki-codes/kadai.git
cd kadai/kadai_games
dotnet restore kadai_games.sln

cd kadai_games.client
npm ci
cd ..
```

接続文字列の設定キーは `ConnectionStrings:DefaultConnection` です。手元の開発用 SQL Server に合わせて、環境変数などで設定してください。認証情報をリポジトリへコミットしないでください。

`kadai_games.Server/appsettings.example.json` に Windows の統合認証を使う LocalDB の設定例を収録しています。このファイルは ASP.NET Core によって自動読み込みされるものではありません。下記の環境変数、または手元の設定ファイルへ必要な項目を設定してください。

PowerShell で LocalDB を使用する例です。

```powershell
$env:ConnectionStrings__DefaultConnection = 'Server=(localdb)\MSSQLLocalDB;Database=KadaiGamesDevelopment;Trusted_Connection=True;MultipleActiveResultSets=true;'
dotnet dev-certs https --trust
dotnet ef database update --project kadai_games.Server
dotnet run --project kadai_games.Server --launch-profile https
```

`dotnet ef` には .NET 8 系の EF CLI ツールが必要です。サーバーは SPA Proxy により `npm start` を起動する構成です。

| 用途 | 設定上の URL |
| --- | --- |
| Angular | `https://localhost:51053` |
| API | `https://localhost:7094` |
| Swagger | `https://localhost:7094/swagger` |

初期ユーザーの自動投入処理はありません。ローカルで API を起動後、Swagger の `POST /api/User/register` から検証用ユーザーを作成してログインします。実在する個人情報や本番用パスワードは使用しないでください。

### ビルドとテスト

```powershell
# kadai_games/ から実行
dotnet build kadai_games.sln
dotnet test kadai_games.sln

# kadai_games/kadai_games.client/ から実行
npm run build
npm test -- --watch=false --browsers=ChromeHeadless
```

一部のサーバーテストは LocalDB の `MyDatabase` / `MyDatabase02` に接続してデータを変更します。業務用データを含まない専用の検証環境で実行してください。上記は構成から整理した実行手順で、すべての環境での成功を保証するものではありません。

整備時には .NET SDK 8.0.413 でビルドを試行しましたが、NuGet のサービスインデックスへの接続に失敗したため、コンパイル・テストの完了は確認できていません。

## 現在の制約と改善予定

- Angular の画面ガードと管理者メニューは実装していますが、CRUD API 全体の認証・管理者権限チェックは整備が必要です。公開サーバーで運用する前に、サーバー側の認可と Cookie 認証に対する CSRF 対策を追加する必要があります。
- API の接続先がクライアント内で固定されています。`src/proxy.conf.js` にも HTTP / HTTPS 設定の不一致があり、環境別設定への集約が改善点です。
- Angular 本体は 18 系、Material / CDK は 17 系です。ロックファイル上の Material / CDK 17.3.10 は Angular 18 をサポート範囲に含みます。更新時はこの組み合わせを含めて動作確認します。
- 初期データ投入、テスト用 DB の分離、CI によるビルド・テストの自動化は今後の改善点です。

## ライセンス

このリポジトリにはライセンスファイルを設定していません。再利用条件は別途確認してください。
