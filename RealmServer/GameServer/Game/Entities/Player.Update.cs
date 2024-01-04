using Common;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities
{
    public partial class Player
    {
        public const int SIGHT_RADIUS = 16;
        public const int SIGHT_RADIUS_SQR = SIGHT_RADIUS * SIGHT_RADIUS;
        public const int ACTIVE_RADIUS = 1; // Activate surrounding chunks

        private readonly HashSet<WorldTile> _visibleTiles = new HashSet<WorldTile>();
        private readonly HashSet<WorldTile> _tilesDiscovered = new HashSet<WorldTile>();
        private readonly List<WorldTile> _newTiles = new List<WorldTile>();

        private readonly ConcurrentQueue<Entity> _deadEntities = new ConcurrentQueue<Entity>();
        private readonly HashSet<Entity> _visibleEntities = new HashSet<Entity>();
        private readonly List<ObjectData> _newEntities = new List<ObjectData>();
        private readonly List<ObjectDropData> _oldEntities = new List<ObjectDropData>();

        private MapChunk _chunk;

        public override bool Move(float posX, float posY)
        {
            if (User.State != ConnectionState.Ready || User.GameInfo.State != GameState.Playing)
                return false;

            if (!base.Move(posX, posY))
                return false;

            if (_chunk != Tile.Chunk)
            {
                if (_chunk != null)
                    for (var cY = _chunk.CY - ACTIVE_RADIUS; cY <= _chunk.CY + ACTIVE_RADIUS; cY++) // Decrease activity of old nearby chunks
                        for (var cX = _chunk.CX - ACTIVE_RADIUS; cX <= _chunk.CX + ACTIVE_RADIUS; cX++)
                        {
                            var chunk = World.Map.Chunks[cX, cY];
                            if (chunk != null)
                            {
                                chunk.ActivityDown();

                                if (chunk.DistSqr(Tile.Chunk) >= 2 * 2) // Distance higher than or equal to 2 chunks (squared)
                                    lock (chunk.Entities)
                                        foreach (var en in chunk.Entities)
                                            if (IsEntityVisible(en))
                                                _deadEntities.Enqueue(en);
                            }
                        }

                _chunk = Tile.Chunk; // Update current chunk

                for (var cY = _chunk.CY - ACTIVE_RADIUS; cY <= _chunk.CY + ACTIVE_RADIUS; cY++) // Increase activity of new nearby chunks
                    for (var cX = _chunk.CX - ACTIVE_RADIUS; cX <= _chunk.CX + ACTIVE_RADIUS; cX++)
                        World.Map.Chunks[cX, cY]?.ActivityUp();
            }

            return true;
        }

        private void SendUpdate()
        {
            _newTiles.Clear();
            _newEntities.Clear();
            _oldEntities.Clear();

            GetNewTiles();
            GetEntities();

            if (_newTiles.Count > 0 || _newEntities.Count > 0 || _oldEntities.Count > 0)
                Update.Write(User.Network,
                    _newTiles,
                    _newEntities,
                    _oldEntities
                    );
        }

        private void GetNewTiles()
        {
            _visibleTiles.Clear();
            switch (World.Config.Blocksight)
            {
                case World.UNBLOCKED_SIGHT:
                    var pX = (int)Position.X;
                    var pY = (int)Position.Y;
                    var width = World.Map.Width;
                    var height = World.Map.Height;
                    for (var y = pY - SIGHT_RADIUS; y <= pY + SIGHT_RADIUS; y++)
                        for (var x = pX - SIGHT_RADIUS; x <= pX + SIGHT_RADIUS; x++)
                            if (x >= 0 && x < width && y >= 0 && y < height && this.TileDistSqr(x, y) <= SIGHT_RADIUS_SQR)
                            {
                                var tile = World.Map[x, y];

                                _visibleTiles.Add(tile);
                                if (_tilesDiscovered.Add(tile)) // This is a newly discovered tile
                                    _newTiles.Add(tile);
                            }
                    break;
            }
        }

        public void TileUpdate(WorldTile tile)
        {
            _tilesDiscovered.Remove(tile);
        }

        public void GetEntities()
        {
            foreach (var kvp in World.Players)
            {
                var plr = kvp.Value;
                if (!plr.Dead && _visibleEntities.Add(plr))
                {
                    plr.DeathEvent += HandleEntityDeath;
                    plr.Stats.StatChangedEvent += HandleEntityStatChanged;
                    _newEntities.Add(plr.Stats.GetObjectData());
                }
            }

            for (var cY = Tile.Chunk.CY - ACTIVE_RADIUS; cY <= Tile.Chunk.CY + ACTIVE_RADIUS; cY++)
                for (var cX = Tile.Chunk.CX - ACTIVE_RADIUS; cX <= Tile.Chunk.CX + ACTIVE_RADIUS; cX++)
                {
                    var chunk = World.Map.Chunks[cX, cY];
                    if (chunk != null)
                    {
                        lock (chunk.Entities)
                            foreach (var en in chunk.Entities)
                            {
                                if (en.IsPlayer)
                                    continue;

                                var visible = IsEntityVisible(en);
                                if (visible && _visibleEntities.Add(en))
                                {
                                    en.DeathEvent += HandleEntityDeath;
                                    en.Stats.StatChangedEvent += HandleEntityStatChanged;
                                    _newEntities.Add(en.Stats.GetObjectData());
                                }
                                if (!visible)
                                {
                                    if (_visibleEntities.Remove(en))
                                    {
                                        en.DeathEvent -= HandleEntityDeath;
                                        en.Stats.StatChangedEvent -= HandleEntityStatChanged;
                                        _oldEntities.Add(en.Stats.GetObjectDropData());
                                    }
                                }
                            }
                    }
                }

            while (_deadEntities.TryDequeue(out var en) && _visibleEntities.Remove(en))
            {
                en.DeathEvent -= HandleEntityDeath;
                en.Stats.StatChangedEvent -= HandleEntityStatChanged;
                _oldEntities.Add(en.Stats.GetObjectDropData());
            }
        }

        private bool IsEntityVisible(Entity en)
        {
            if (en.IsPlayer)
                return true;

            if (_visibleTiles.Contains(en.Tile))
                return true;

            return false;
        }

        private void HandleEntityDeath(Entity en)
        {
            if (Dead)
                return;

            _deadEntities.Enqueue(en);
        }
    }
}
