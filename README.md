# Extreal.Integration.P2P.WebRTC

## How to test

- Enter the following command in the `WebScripts~` directory.
   ```bash
   $ yarn
   $ yarn dev
   ```
- Import the sample MVS from Package Manager.
- Enter the following command in the `MVS/WebScripts` directory.
   ```bash
   $ yarn
   $ yarn dev
   ```
   The JavaScript code will be built and output to `/Assets/WebTemplates/Dev`.
- Open `Build Settings` and change the platform to `WebGL`.
- Select `Dev` from `Player Settings > Resolution and Presentation > WebGL Template`.
- Add all scenes in MVS to `Scenes In Build`.
- See [README](SignalingServer~/README.md) to start a signaling server.
- Play
  - Native
    - Open multiple Unity editors using ParrelSync.
    - Run
      - Scene: MVS/App/App
  - WebGL
    - Run from `Build And Run`.

## Test cases for manual testing

### ホスト

- グループ選択画面
  - 名前を指定してグループを作れること（ホストの開始）
  - 名前が重複した場合はエラー通知されること（ホスト名の重複）
  - 接続できずグループを作れない場合はエラー通知されること（接続失敗）
- バーチャル空間
  - クライアントがグループに参加できること（クライアントの参加）
  - クライアントがグループから退出できること（クライアントの退出）
  - グループ選択画面に戻れること（ホストの停止）
  - 接続が中断した場合はエラー通知されること（接続中断）

### クライアント

- グループ選択画面
  - グループ一覧を取得できること（ホスト一覧の取得）
  - グループに参加できること（ホストに参加）
  - 接続できずグループ一覧を取得できない場合はエラー通知されること（接続失敗）
- バーチャル空間
  - グループ選択画面に戻れること（ホストから退出） 
  - 接続が中断した場合はエラー通知されること（接続中断）
