using Common;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game.Chat;
using GameServer.Game.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameServer.Game.Worlds
{
    //The mad god who look after the realm...
    public class Oryx
    {
        private readonly Logger _log = new Logger(typeof(Oryx));
        private readonly Random _rand = new Random();

        #region Taunt data

        private struct TauntData
        {
            public string[] Spawn;
            public string[] NumberOfEnemies;
            public string[] Final;
            public string[] Killed;
        }

        private static readonly Tuple<string, TauntData>[] EnemyTaunts =
        {
            Tuple.Create("Lich", new TauntData
            {
                NumberOfEnemies = new[]
                {
                    "I am invincible while my {COUNT} Liches still stand!",
                    "My {COUNT} Liches will feast on your essence!"
                },
                Final = new[]
                {
                    "My final Lich shall consume your souls!",
                    "My final Lich will protect me forever!"
                }
            }),
            Tuple.Create("Ent Ancient", new TauntData
            {
                NumberOfEnemies = new[]
                {
                    "Mortal scum! My {COUNT} Ent Ancients will defend me forever!",
                    "My forest of {COUNT} Ent Ancients is all the protection I need!"
                },
                Final = new[]
                {
                    "My final Ent Ancient will destroy you all!",
                    "My final Ent Ancient shall crush you!"
                }
            }),
            Tuple.Create("Oasis Giant", new TauntData
            {
                NumberOfEnemies = new[]
                {
                    "My {COUNT} Oasis Giants will feast on your flesh!",
                    "You have no hope against my {COUNT} Oasis Giants!"
                },
                Final = new[]
                {
                    "A powerful Oasis Giant still fights for me!",
                    "You will never defeat me while an Oasis Giant remains!"
                }
            }),
            Tuple.Create("Phoenix Lord", new TauntData
            {
                NumberOfEnemies = new[]
                {
                    "Maggots! My {COUNT} Phoenix Lord will burn you to ash!",
                    "My {COUNT} Phoenix Lords will serve me forever!"
                },
                Final = new[]
                {
                    "My final Phoenix Lord will never fall!",
                    "My last Phoenix Lord will blacken your bones!"
                }
            }),
            Tuple.Create("Ghost King", new TauntData
            {
                NumberOfEnemies = new[]
                {
                    "My {COUNT} Ghost Kings give me more than enough protection!",
                    "Pathetic humans! My {COUNT} Ghost Kings shall destroy you utterly!"
                },
                Final = new[]
                {
                    "A mighty Ghost King remains to guard me!",
                    "My final Ghost King is untouchable!"
                }
            }),
            Tuple.Create("Cyclops God", new TauntData
            {
                NumberOfEnemies = new[]
                {
                    "Cretins! I have {COUNT} Cyclops Gods to guard me!",
                    "My {COUNT} powerful Cyclops Gods will smash you!"
                },
                Final = new[]
                {
                    "My last Cyclops God will smash you to pieces!",
                    "My final Cyclops God shall crush your puny skulls!"
                }
            }),
            Tuple.Create("Red Demon", new TauntData
            {
                NumberOfEnemies = new[]
                {
                    "Fools! There is no escape from my {COUNT} Red Demons!",
                    "My legion of {COUNT} Red Demons live only to serve me!"
                },
                Final = new[]
                {
                    "My final Red Demon is unassailable!",
                    "A Red Demon still guards me!"
                }
            }),

            Tuple.Create("Skull Shrine", new TauntData
            {
                Spawn = new[]
                {
                    "Your futile efforts are no match for a Skull Shrine!"
                },
                NumberOfEnemies = new[]
                {
                    "Insects!  {COUNT} Skull Shrines still protect me",
                    "You hairless apes will never overcome my {COUNT} Skull Shrines!",
                    "You frail humans will never defeat my {COUNT} Skull Shrines!",
                    "Miserable worms like you cannot stand against my {COUNT} Skull Shrines!",
                    "Imbeciles! My {COUNT} Skull Shrines make me invincible!"
                },
                Final = new[]
                {
                    "Pathetic fools!  A Skull Shrine guards me!",
                    "Miserable scum!  My Skull Shrine is invincible!"
                },
                Killed = new[]
                {
                    "You defaced a Skull Shrine!  Minions, to arms!",
                    "{PLAYER} razed one of my Skull Shrines -- I WILL HAVE MY REVENGE!",
                    "{PLAYER}, you will rue the day you dared to defile my Skull Shrine!",
                    "{PLAYER}, you contemptible pig! Ruining my Skull Shrine will be the last mistake you ever make!",
                    "{PLAYER}, you insignificant cur! The penalty for destroying a Skull Shrine is death!"
                }
            }),
            Tuple.Create("Cube God", new TauntData
            {
                Spawn = new[]
                {
                    "Your meager abilities cannot possibly challenge a Cube God!"
                },
                NumberOfEnemies = new[]
                {
                    "Filthy vermin! My {COUNT} Cube Gods will exterminate you!",
                    "Loathsome slugs! My {COUNT} Cube Gods will defeat you!",
                    "You piteous cretins! {COUNT} Cube Gods still guard me!",
                    "Your pathetic rabble will never survive against my {COUNT} Cube Gods!",
                    "You feeble creatures have no hope against my {COUNT} Cube Gods!"
                },
                Final = new[]
                {
                    "Worthless mortals! A mighty Cube God defends me!",
                    "Wretched mongrels!  An unconquerable Cube God is my bulwark!"
                },
                Killed = new[]
                {
                    "You have dispatched my Cube God, but you will never escape my Realm!",
                    "{PLAYER}, you pathetic swine! How dare you assault my Cube God?",
                    "{PLAYER}, you wretched dog! You killed my Cube God!",
                    "{PLAYER}, you may have destroyed my Cube God but you will never defeat me!",
                    "I have many more Cube Gods, {PLAYER}!"
                }
            }),
            Tuple.Create("Pentaract", new TauntData
            {
                Spawn = new[]
                {
                    "Behold my Pentaract, and despair!"
                },
                NumberOfEnemies = new[]
                {
                    "Wretched creatures! {COUNT} Pentaracts remain!",
                    "You detestable humans will never defeat my {COUNT} Pentaracts!",
                    "My {COUNT} Pentaracts will protect me forever!",
                    "Your weak efforts will never overcome my {COUNT} Pentaracts!",
                    "Defiance is useless! My {COUNT} Pentaracts will crush you!"
                },
                Final = new[]
                {
                    "I am invincible while my Pentaract stands!",
                    "Ignorant fools! A Pentaract guards me still!"
                },
                Killed = new[]
                {
                    "That was but one of many Pentaracts!",
                    "You have razed my Pentaract, but you will die here in my Realm!",
                    "{PLAYER}, you lowly scum!  You'll regret that you ever touched my Pentaract!",
                    "{PLAYER}, you flea-ridden animal! You destoryed my Pentaract!",
                    "{PLAYER}, by destroying my Pentaract you have sealed your own doom!"
                }
            }),
            Tuple.Create("Grand Sphinx", new TauntData
            {
                Spawn = new[]
                {
                    "At last, a Grand Sphinx will teach you to respect!"
                },
                NumberOfEnemies = new[]
                {
                    "You dull-spirited apes! You shall pose no challenge for {COUNT} Grand Sphinxes!",
                    "Regret your choices, blasphemers! My {COUNT} Grand Sphinxes will teach you respect!",
                    "My {COUNT} Grand Sphinxes protect my Chamber with their lives!",
                    "My Grand Sphinxes will bewitch you with their beauty!"
                },
                Final = new[]
                {
                    "A Grand Sphinx is more than a match for this rabble.",
                    "You festering rat-catchers! A Grand Sphinx will make you doubt your purpose!",
                    "Gaze upon the beauty of the Grand Sphinx and feel your last hopes drain away."
                },
                Killed = new[]
                {
                    "The death of my Grand Sphinx shall be avenged!",
                    "My Grand Sphinx, she was so beautiful. I will kill you myself, {PLAYER}!",
                    "My Grand Sphinx had lived for thousands of years! You, {PLAYER}, will not survive the day!",
                    "{PLAYER}, you up-jumped goat herder! You shall pay for defeating my Grand Sphinx!",
                    "{PLAYER}, you pestiferous lout! I will not forget what you did to my Grand Sphinx!",
                    "{PLAYER}, you foul ruffian! Do not think I forget your defiling of my Grand Sphinx!"
                }
            }),
            Tuple.Create("Lord of the Lost Lands", new TauntData
            {
                Spawn = new[]
                {
                    "Cower in fear of my Lord of the Lost Lands!",
                    "My Lord of the Lost Lands will make short work of you!"
                },
                NumberOfEnemies = new[]
                {
                    "Cower before your destroyer! You stand no chance against {COUNT} Lords of the Lost Lands!",
                    "Your pathetic band of fighters will be crushed under the might feet of my {COUNT} Lords of the Lost Lands!",
                    "Feel the awesome might of my {COUNT} Lords of the Lost Lands!",
                    "Together, my {COUNT} Lords of the Lost Lands will squash you like a bug!",
                    "Do not run! My {COUNT} Lords of the Lost Lands only wish to greet you!"
                },
                Final = new[]
                {
                    "Give up now! You stand no chance against a Lord of the Lost Lands!",
                    "Pathetic fools! My Lord of the Lost Lands will crush you all!",
                    "You are nothing but disgusting slime to be scraped off the foot of my Lord of the Lost Lands!"
                },
                Killed = new[]
                {
                    "How dare you foul-mouthed hooligans treat my Lord of the Lost Lands with such indignity!",
                    "What trickery is this?! My Lord of the Lost Lands was invincible!",
                    "You win this time, {PLAYER}, but mark my words:  You will fall before the day is done.",
                    "{PLAYER}, I will never forget you exploited my Lord of the Lost Lands' weakness!",
                    "{PLAYER}, you have done me a service! That Lord of the Lost Lands was not worthy of serving me.",
                    "You got lucky this time {PLAYER}, but you stand no chance against me!"
                }
            }),
            Tuple.Create("Hermit God", new TauntData
            {
                Spawn = new[]
                {
                    "My Hermit God's thousand tentacles shall drag you to a watery grave!"
                },
                NumberOfEnemies = new[]
                {
                    "You will make a tasty snack for my Hermit Gods!",
                    "I will enjoy watching my {COUNT} Hermit Gods fight over your corpse!"
                },
                Final = new[]
                {
                    "You will be pulled to the bottom of the sea by my mighty Hermit God.",
                    "Flee from my Hermit God, unless you desire a watery grave!",
                    "My Hermit God awaits more sacrifices for the majestic Thessal.",
                    "My Hermit God will pull you beneath the waves!",
                    "You will make a tasty snack for my Hermit God!"
                },
                Killed = new[]
                {
                    "This is preposterous!  There is no way you could have defeated my Hermit God!",
                    "You were lucky this time, {PLAYER}!  You will rue this day that you killed my Hermit God!",
                    "You naive imbecile, {PLAYER}! Without my Hermit God, Dreadstump is free to roam the seas without fear!",
                    "My Hermit God was more than you'll ever be, {PLAYER}. I will kill you myself!"
                }
            }),
            Tuple.Create("Ghost Ship", new TauntData
            {
                Spawn = new[]
                {
                    "My Ghost Ship will terrorize you pathetic peasants!",
                    "A Ghost Ship has entered the Realm."
                },
                Final = new[]
                {
                    "My Ghost Ship will send you to a watery grave.",
                    "You filthy mongrels stand no chance against my Ghost Ship!",
                    "My Ghost Ship's cannonballs will crush your pathetic Knights!"
                },
                Killed = new[]
                {
                    "My Ghost Ship will return!",
                    "Alas, my beautiful Ghost Ship has sunk!",
                    "{PLAYER}, you foul creature.  I shall see to your death personally!",
                    "{PLAYER}, has crossed me for the last time! My Ghost Ship shall be avenged.",
                    "{PLAYER} is such a jerk!",
                    "How could a creature like {PLAYER} defeat my dreaded Ghost Ship?!",
                    "The spirits of the sea will seek revenge on your worthless soul, {PLAYER}!"
                }
            }),
            Tuple.Create("Dragon Head", new TauntData
            {
                Spawn = new[]
                {
                    "The Rock Dragon has been summoned.",
                    "Beware my Rock Dragon. All who face him shall perish."
                },
                Final = new[]
                {
                    "My Rock Dragon will end your pathetic existence!",
                    "Fools, no one can withstand the power of my Rock Dragon!",
                    "The Rock Dragon will guard his post until the bitter end.",
                    "The Rock Dragon will never let you enter the Lair of Draconis."
                },
                Killed = new[]
                {
                    "My Rock Dragon will return!",
                    "The Rock Dragon has failed me!",
                    "{PLAYER} knows not what he has done.  That Lair was guarded for the Realm's own protection!",
                    "{PLAYER}, you have angered me for the last time!",
                    "{PLAYER} will never survive the trials that lie ahead.",
                    "A filthy weakling like {PLAYER} could never have defeated my Rock Dragon!!!",
                    "You shall not live to see the next sunrise, {PLAYER}!"
                }
            }),
            Tuple.Create("shtrs Defense System", new TauntData
            {
                Spawn = new[]
                {
                    "The Shatters has been discovered!?!",
                    "The Forgotten King has raised his Avatar!"
                },
                Final = new[]
                {
                    "Attacking the Avatar of the Forgotten King would be...unwise.",
                    "Kill the Avatar, and you risk setting free an abomination.",
                    "Before you enter the Shatters you must defeat the Avatar of the Forgotten King!"
                },
                Killed = new[]
                {
                    "The Avatar has been defeated!",
                    "How could simpletons kill The Avatar of the Forgotten King!?",
                    "{PLAYER} has unleashed an evil upon this Realm.",
                    "{PLAYER}, you have awoken the Forgotten King. Enjoy a slow death!",
                    "{PLAYER} will never survive what lies in the depths of the Shatters.",
                    "Enjoy your little victory while it lasts, {PLAYER}!"
                }
            })
        };

        #endregion

        private readonly Dictionary<TerrainType, float> _maxTerrainEnemies = new Dictionary<TerrainType, float>()
        {
            { TerrainType.Mountains, 0.015f },
            { TerrainType.HighSand, 0.015f },
            { TerrainType.HighPlains, 0.015f },
            { TerrainType.HighForest, 0.015f },
            { TerrainType.MidSand, 0.015f },
            { TerrainType.MidPlains, 0.015f },
            { TerrainType.MidForest, 0.015f },
            { TerrainType.LowSand, 0.015f },
            { TerrainType.LowPlains, 0.015f },
            { TerrainType.LowForest, 0.015f },
            { TerrainType.ShoreSand, 0.015f },
            { TerrainType.ShorePlains, 0.015f },
        };

        public Entity ActiveEvent { get; private set; }

        private readonly RealmConfig _config;
        private readonly Realm _realm;
        private readonly int[] _terrainEnemyCount;
        private readonly long[] _lastTicks;
        private readonly Dictionary<string, int> _spawnedEvents; // Counts the amount of times a specific event has spawned

        public Oryx(Realm realm)
        {
            _realm = realm;
            _config = RealmConfig.Config;
            _terrainEnemyCount = new int[12];
            _lastTicks = new long[2];

            _spawnedEvents = new Dictionary<string, int>();
            foreach (var evt in _config.Events)
                if (evt.PerRealmMax != -1)
                    _spawnedEvents.TryAdd(evt.ObjectId, 0);
        }

        public void Initialize()
        {
            Populate();
        }

        public void Tick(RealmTime time)
        {
            if (!_realm.Closed)
            {
                if (time.TotalElapsedMs - _lastTicks[0] > 25000) // 25 seconds
                {
                    _lastTicks[0] = time.TotalElapsedMs;
                    Repopulate();
                }

                if (time.TotalElapsedMs - _lastTicks[1] > 35000)
                {
                    _lastTicks[1] = time.TotalElapsedMs;
                    OryxTaunt();
                }
            }
        }

        private void OryxTaunt()
        {
            var count = 0;
            var taunt = EnemyTaunts[_rand.Next(0, EnemyTaunts.Length)];
            foreach (var kvp in _realm.Enemies)
            {
                var desc = kvp.Value.Desc;
                if (desc == null || desc.ObjectId != taunt.Item1)
                    continue;
                count++;
            }

            if (count == 0)
                return;

            string msg = null;
            if ((count == 1 && taunt.Item2.Final != null) ||
                (taunt.Item2.Final != null && taunt.Item2.NumberOfEnemies == null))
            {
                var arr = taunt.Item2.Final;
                msg = arr[_rand.Next(0, arr.Length)];
            }
            else
            {
                string[] arr = taunt.Item2.NumberOfEnemies;
                if (arr != null)
                    msg = arr[_rand.Next(0, arr.Length)].Replace("{COUNT}", count.ToString());
            }

            ChatManager.Oryx(_realm, msg);
        }

        private void Populate()
        {
            var terrainCount = new int[12];
            var w = _realm.Map.Width;
            var h = _realm.Map.Height;
            for (var y = 0; y < h; y++) // Count the number of tiles per terrain
                for (var x = 0; x < w; x++)
                {
                    var tile = _realm.Map[x, y];
                    if (tile.Data.Terrain != TerrainType.None)
                        terrainCount[(int)tile.Data.Terrain - 1]++;
                }

            var totalCount = 0;
            foreach (var kvp in XmlLibrary.TerrainEnemies) // Spawn enemies by terrain type
            {
                var enCount = 0;
                var terrain = kvp.Key;
                var index = (int)terrain - 1;
                var maxCount = (int)(terrainCount[index] * _maxTerrainEnemies[terrain]); // Calculate maximum amount of enemies
                while (enCount < maxCount)
                    foreach (var desc in kvp.Value)
                    {
                        if (_rand.NextDouble() > desc.SpawnProb)
                            continue;

                        enCount += SpawnTerrainEnemy(desc, terrain);
                    }

                _maxTerrainEnemies[terrain] = maxCount; // Replace the percentage with the calculated maximum amount of admitted enemies

                totalCount += enCount;
            }

            bool success;
            RealmEventData eventData;
            do
            {
                eventData = _config.Events.RandomElement();
                success = XmlLibrary.Id2Object(eventData.ObjectId) != null &&
                    (eventData.PerRealmMax == -1 || (eventData.PerRealmMax != -1 && eventData.PerRealmMax > _spawnedEvents[eventData.ObjectId]));
            }
            while (!success);

            var eventTerrain = (TerrainType)_rand.Next(1, 11); // From 1 to 10
            SpawnRealmEvent(eventData, eventTerrain);

            _log.Info($"Spawned {totalCount} entities in the {_realm.DisplayName} Realm.");
        }

        private void Repopulate()
        {
            foreach (var kvp in _realm.Enemies) // Count enemies by terrain type
            {
                var enemy = kvp.Value;
                if (enemy.Dead || enemy.Desc.Terrain == TerrainType.None)
                    continue;

                var terrainIndex = (int)enemy.Desc.Terrain - 1;
                _terrainEnemyCount[terrainIndex]++;
            }

            var totalCount = 0;
            foreach (var kvp in _maxTerrainEnemies)
            {
                var maxCount = kvp.Value;
                var terrainIndex = (int)kvp.Key - 1;
                while (_terrainEnemyCount[terrainIndex] < maxCount) // If the current enemy count is lower than the max, spawn more enemies
                    foreach (var desc in XmlLibrary.TerrainEnemies[kvp.Key])
                    {
                        if (_rand.NextDouble() > desc.SpawnProb)
                            continue;

                        totalCount += _terrainEnemyCount[terrainIndex] += SpawnTerrainEnemy(desc, kvp.Key);
                    }
            }

            if (ActiveEvent == null || ActiveEvent.Dead) // If current event is dead, spawn another
            {
                var terrain = (TerrainType)_rand.Next(1, 11); // From 1 to 10
                SpawnRealmEvent(_config.Events.RandomElement(), terrain);
            }

            _log.Debug($"Respawned {totalCount} entities in the {_realm.DisplayName} Realm.");
        }

        private int SpawnTerrainEnemy(ObjectDesc desc, TerrainType terrain)
        {
            int num = 1;
            if (desc.Spawn != null)
            {
                num = (int)GetNormal(_rand, desc.Spawn.Mean, desc.Spawn.Deviation);

                if (num > desc.Spawn.Max)
                    num = desc.Spawn.Max;
                else if (num < desc.Spawn.Min)
                    num = desc.Spawn.Min;
            }

            var pt = new IntPoint();
            do
            {
                pt.X = _rand.Next(0, _realm.Map.Width);
                pt.Y = _rand.Next(0, _realm.Map.Height);
            } while (_realm.Map[pt.X, pt.Y].Data.Terrain != terrain ||
                     !_realm.IsPassable(pt.X, pt.Y));

            for (var k = 0; k < num; k++)
            {
                var entity = Entity.Resolve(desc.ObjectType);
                entity.Move(
                    pt.X + (float)(_rand.NextDouble() * 2 - 1) * 5,
                    pt.Y + (float)(_rand.NextDouble() * 2 - 1) * 5);

                //if (entity is Enemy e1) e1.Terrain = terrain;
                entity.EnterWorld(_realm);
            }
            return num;
        }

        private void SpawnRealmEvent(RealmEventData eventData, TerrainType terrain)
        {
            var eventDesc = XmlLibrary.Id2Object(eventData.ObjectId);
            if (eventDesc != null)
            {
                var pt = new IntPoint();
                do
                {
                    pt.X = _rand.Next(0, _realm.Map.Width);
                    pt.Y = _rand.Next(0, _realm.Map.Height);
                } while (_realm.Map[pt.X, pt.Y].Data.Terrain != terrain ||
                         !_realm.IsPassable(pt.X, pt.Y));

                ActiveEvent = _realm.SpawnSetPiece(eventData.SetPiece, pt.X, pt.Y, eventId: eventDesc.ObjectId);

                var tauntData = EnemyTaunts.FirstOrDefault(t => t.Item1 == eventDesc.ObjectId); // Spawn announcement
                if (tauntData != null)
                    ChatManager.Oryx(_realm, tauntData.Item2.Spawn.RandomElement());

                if (_spawnedEvents.TryGetValue(eventDesc.ObjectId, out var count))
                    _spawnedEvents[eventDesc.ObjectId] += 1;

                _log.Info($"Spawned {eventDesc.ObjectId} in the {_realm.DisplayName} Realm.");
            }
        }

        private static double GetUniform(Random rand)
        {
            // 0 <= u < 2^32
            var u = (uint)(rand.NextDouble() * uint.MaxValue);
            // The magic number below is 1/(2^32 + 2).
            // The result is strictly between 0 and 1.
            return (u + 1.0) * 2.328306435454494e-10;
        }

        private static double GetNormal(Random rand)
        {
            // Use Box-Muller algorithm
            var u1 = GetUniform(rand);
            var u2 = GetUniform(rand);
            var r = Math.Sqrt(-2.0 * Math.Log(u1));
            var theta = 2.0 * Math.PI * u2;
            return r * Math.Sin(theta);
        }

        private static double GetNormal(Random rand, double mean, double standardDeviation)
        {
            return mean + standardDeviation * GetNormal(rand);
        }
    }
}
