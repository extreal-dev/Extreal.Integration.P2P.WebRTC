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
  - Logs are output to the console in the following order when client join

    ```log
    Error: CreatePeerClient Error Test
    Error: CreatePeerClient Async Error Test
    Dummy createPcAsync Start
    Dummy createPcAsync Finish
    New DataChannel: id=XXXXXXXXXXXXXXXXXXXX label=sample
    Error: CreatePeerClient Error Test
    Error: CreatePeerClient Async Error Test
    Dummy createPcAsync Start
    Dummy createPcAsync Finish
    ```

  - Two clients can join a group (multiple client join)
  - When clients connect, the socket ID of each connected client is displayed
  - Clients can leave the group (client exit)
  - When clients disconnect, the socket ID of each connected client is displayed
  - Logs are output to the console in the following order when client leave

    ```log
    Error: ClosePeerClient Error Test
    Error: ClosePeerClient Error Test
    ```

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
  - Logs are output to the console in the following order when start

      ```log
      Error: CreatePeerClient Error Test
      Error: CreatePeerClient Async Error Test
      Dummy createPcAsync Start
      Dummy createPcAsync Finish
      Error: CreatePeerClient Error Test
      Error: CreatePeerClient Async Error Test
      Dummy createPcAsync Start
      Dummy createPcAsync Finish
      ...
      New DataChannel: id=XXXXXXXXXXXXXXXXXXXX label=sample
      ```

  - When a client connects, the socket ID of the client is displayed
  - Logs are output to the console in the following order when another client join

    ```log
    Error: CreatePeerClient Error Test
    Error: CreatePeerClient Async Error Test
    Dummy createPcAsync Start
    Dummy createPcAsync Finish
    New DataChannel: id=XXXXXXXXXXXXXXXXXXXX label=sample
    Error: CreatePeerClient Error Test
    Error: CreatePeerClient Async Error Test
    Dummy createPcAsync Start
    Dummy createPcAsync Finish
    ```

  - When a client disconnects, the socket ID of the client is displayed
  - Ability to return to the group selection screen (leave host)
  - Logs are output to the console in the following order when other client leave

    ```log
    Error: ClosePeerClient Error Test
    Error: ClosePeerClient Error Test
    ```

  - Error notification if connection is interrupted (connection interrupted)
  - Error notification if P2P start processing is timed out (P2P start failed)
