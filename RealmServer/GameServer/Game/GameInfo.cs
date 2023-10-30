﻿using Common.Database;
using GameServer.Game.Logic;
using GameServer.Game.Logic.Entities;
using GameServer.Game.Logic.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public enum GameState
    {
        Idle, // User has established connection to server but hasn't loaded to any world yet
        Loading, // User has sent Hello packet, and now we're waiting for client to send Load packet
        Playing // User has established
    }

    public class GameInfo
    {
        public readonly User User;
        public World World { get; private set; }
        public Player Player { get; private set; }
        public GameState State { get; private set; }

        public GameInfo(User user)
        {
            User = user;
        }

        public void SetWorld(int worldId)
        {
            State = GameState.Loading;
            World = RealmManager.GetWorld(worldId);
        }

        public void Load(DbChar chr, World world)
        {
            State = GameState.Playing;

            Player = new Player(User, chr);
            Player.EnterWorld(world);
        }

        public void Unload()
        {
            State = GameState.Idle;

            Player.Death();
        }

        public void Reset()
        {
            Player = null;
            World = null;
            State = GameState.Idle;
        }
    }
}