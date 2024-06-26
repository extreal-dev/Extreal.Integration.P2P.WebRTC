# Extreal.Integration.P2P.WebRTC

## How to test

- Enter the following command in the `WebScripts~` directory.
   ```bash
   $ yarn
   $ yarn dev
   ```
- Import the sample MVS from Package Manager.
- Enter the following command in the `MVS/WebGLScripts~` directory.
   ```bash
   $ yarn
   $ yarn dev
   ```
   The JavaScript code will be built and output to `/Assets/WebGLTemplates/Dev`.
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
    - See [README](https://github.com/extreal-dev/Extreal.Dev/blob/main/WebGLBuild/README.md) to run WebGL application in local environment.

## Test cases for manual testing

### Host

- Group selection screen
- Ability to create a group by specifying a name (host start)
  - Error notification if a name is duplicated (duplicate host name)
  - Error notification if a group cannot be created because a connection cannot be made (connection failure)
- Virtual space
  - Own socket ID is displayed when start
  - Client can join a group (client join)
  - Two clients can join a group (multiple client join)
  - The socket ID of each connected client is displayed when clients connect
  - Error handling if an error occurs in hooks on connecting
    - Error test logs are displayed twice in console for each client connection
  - Clients can leave the group (client exit)
  - The socket ID of each disconnected client is displayed when clients disconnect
  - Error handling if an error occurs in hooks on disconnecting
    - Error test logs are displayed twice in console for each client disconnect
  - Ability to return to the group selection screen (host stop)
  - Error notification if connection is interrupted (connection interrupted)

### Client

- Group selection screen
  - Ability to retrieve group list (retrieve host list)
  - Ability to join a group (join host)
  - Error notification if unable to connect and obtain group list (connection failure)
- Virtual space
  - Own socket ID is displayed when start
  - 2nd clients can join a group while still joined (multiple client join)
  - Socket IDs of already connected clients and host are displayed when start
  - The socket ID of each connected client is displayed when other clients connect
  - Error handling if an error occurs in hooks on connecting
    - Error test logs are displayed twice in console for each client connection
  - The socket ID of each disconnected client is displayed when other clients disconnect
  - Error handling if an error occurs in hooks on disconnecting
    - Error test logs are displayed twice in console for each client disconnect
  - Ability to return to the group selection screen (leave host)
  - Error notification if connection is interrupted (connection interrupted)
  - Error notification if P2P start processing is timed out (P2P start failed)
