using Common.Resources.Config;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Logic.Worlds
{
    public class Nexus : World
    {
        private readonly HashSet<WorldTile> _realmPortals = new HashSet<WorldTile>();
        private readonly List<WorldTile> _realmPortalTiles = new List<WorldTile>();

        public Nexus()
            : base(NEXUS, 0, -1)
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            // Load all tiles with RealmPortals region
            foreach (var tile in Map.Tiles)
                if (tile.Region == TileRegion.Realm_Portals)
                    _realmPortalTiles.Add(tile);

            var realmCount = GameServerConfig.Config.RealmCount;
            if (realmCount == 0)
                return;

            // Spawn realm portals
            for (var i = 0; i < realmCount; i++)
            {
                if (_realmPortals.Count >= _realmPortalTiles.Count)
                    break;

                WorldTile tile = null; // Select a random realm portal tile
                while (tile == null || _realmPortals.Contains(tile))
                    tile = _realmPortalTiles.RandomElement();

                var portal = new Portal(0x0704);
                portal.Move(tile.X + 0.5f, tile.Y + 0.5f);
                portal.EnterWorld(this);
            }
        }
    }
}
