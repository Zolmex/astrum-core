﻿using Common;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Net.Messaging;
using GameServer.Game.Net.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Logic.Entities
{
    public partial class Player
    {
        public const int SIGHT_RADIUS = 16;
        public const int SIGHT_RADIUS_SQR = SIGHT_RADIUS * SIGHT_RADIUS;
        private const int ACTIVE_RADIUS = 1; // Activate surrounding chunks

        private readonly HashSet<WorldTile> _visibleTiles = new HashSet<WorldTile>();
        private readonly HashSet<WorldTile> _tilesDiscovered = new HashSet<WorldTile>();
        private readonly List<WorldTile> _newTiles = new List<WorldTile>();

        private readonly ConcurrentQueue<Entity> _deadEntities = new ConcurrentQueue<Entity>();
        private readonly HashSet<Entity> _visibleEntities = new HashSet<Entity>();
        private readonly List<ObjectData> _newEntities = new List<ObjectData>();
        private readonly List<ObjectDropData> _oldEntities = new List<ObjectDropData>();

        private MapChunk _chunk;

        public override void Move(float posX, float posY)
        {
            base.Move(posX, posY);

            if (_chunk != Tile.Chunk)
            {
                if (_chunk == null)
                    _chunk = Tile.Chunk;
                else
                    for (var cY = _chunk.CY - ACTIVE_RADIUS; cY < _chunk.CY + ACTIVE_RADIUS; cY++) // Decrease activity of old nearby chunks
                        for (var cX = _chunk.CX - ACTIVE_RADIUS; cX < _chunk.CX + ACTIVE_RADIUS; cX++)
                            World.Map.Chunks[cX, cY].ActivityDown();

                _chunk = Tile.Chunk; // Update current chunk

                for (var cY = _chunk.CY - ACTIVE_RADIUS; cY < _chunk.CY + ACTIVE_RADIUS; cY++) // Increase activity of new nearby chunks
                    for (var cX = _chunk.CX - ACTIVE_RADIUS; cX < _chunk.CX + ACTIVE_RADIUS; cX++)
                        World.Map.Chunks[cX, cY].ActivityUp();
            }
        }

        private void SendUpdate()
        {
            _newTiles.Clear();
            _newEntities.Clear();
            _oldEntities.Clear();

            GetNewTiles();
            GetEntities();

            if (_newTiles.Count > 0 || _newEntities.Count > 0 || _oldEntities.Count > 0)
                User.SendPacket(PacketId.UPDATE, Update.Write(User,
                    _newTiles,
                    _newEntities,
                    _oldEntities
                    ));
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
                    for (var y = pY - SIGHT_RADIUS; y < pY + SIGHT_RADIUS; y++)
                        for (var x = pX - SIGHT_RADIUS; x < pX + SIGHT_RADIUS; x++)
                            if (x >= 0 && x < width && y >= 0 && y < height && this.TileDistSqr(x, y) <= SIGHT_RADIUS_SQR)
                            {
                                var tile = World.Map[x, y];

                                if (!_tilesDiscovered.Contains(tile)) // This is a newly discovered tile
                                {
                                    _newTiles.Add(tile);
                                    _tilesDiscovered.Add(tile);
                                }
                                _visibleTiles.Add(tile);
                            }
                    break;
            }
        }

        public void GetEntities()
        {
            foreach (var kvp in World.ActiveEntities)
            {
                var en = kvp.Value;
                var visible = IsEntityVisible(en);
                if (visible && _visibleEntities.Add(en))
                {
                    en.DeathEvent += HandleEntityDeath;
                    en.Stats.StatChangedEvent += HandleEntityStatChanged;
                    _newEntities.Add(en.Stats.GetObjectData());
                }
                if (!visible)
                {
                    if (_visibleEntities.Add(en))
                        _visibleEntities.Remove(en);
                    else
                    {
                        en.DeathEvent -= HandleEntityDeath;
                        en.Stats.StatChangedEvent -= HandleEntityStatChanged;
                        _oldEntities.Add(en.Stats.GetObjectDropData());
                        _visibleEntities.Remove(en);
                    }
                }
            }

            while (_deadEntities.TryDequeue(out var en))
            {
                _oldEntities.Add(en.Stats.GetObjectDropData());
                _visibleEntities.Remove(en);
            }
        }

        private bool IsEntityVisible(Entity en)
        {
            if (en is Player)
                return true;

            if (en.IsStandingOn(_visibleTiles))
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