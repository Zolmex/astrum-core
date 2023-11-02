using Common.Database;
using GameServer.Game.Collections;
using GameServer.Game.Entities;
using GameServer.Game.Worlds;
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
        public DbChar Char { get; private set; }
        public Player Player { get; private set; }
        public GameState State { get; private set; }

        public GameInfo(User user)
        {
            User = user;
        }

        public void SetWorld(World world)
        {
            State = GameState.Loading;
            World = world;
        }

        public void Load(DbChar chr, World world)
        {
            State = GameState.Playing;

            Char = chr;
            Player = new Player(User, chr);
            Player.EnterWorld(world);
        }

        public void Unload()
        {
            State = GameState.Idle;

            // Don't set char to null, we need that for reconnecting
            Player.LeaveWorld();
        }

        public void Reset()
        {
            Char = null;
            Player = null;
            World = null;
            State = GameState.Idle;
        }
    }
}
