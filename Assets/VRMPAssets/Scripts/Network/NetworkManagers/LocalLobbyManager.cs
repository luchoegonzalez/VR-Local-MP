using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Unity.Services.Relay;
using Unity.Netcode.Transports.UTP;
using Unity.XR.CoreUtils.Bindings.Variables;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;

namespace XRMultiplayer
{

    public class LocalLobbyManager : LobbyManager
    {

        public new void MockLobby(string HostId)
        {
            connectedLobby = new Lobby(id: "mock", name: "mock", hostId: HostId, lobbyCode: "mock", maxPlayers: 20, isPrivate: false, data: new Dictionary<string, DataObject>());
        }
    }
}
