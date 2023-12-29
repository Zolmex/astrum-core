using Common;
using Common.Control;
using Common.Database;
using Common.Resources.Config;
using Common.Resources.World;
using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game;
using GameServer.Game.Collections;
using GameServer.Game.Entities.Behaviors;
using GameServer.Game.Network;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace GameServer
{
    class Program
    {
        private static readonly Logger Log = new Logger(typeof(Program));

        static void Main(string[] args)
        {
            Console.Title = $"Realm Server v{Assembly.GetExecutingAssembly().GetName().Version} - GameServer";
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Thread.CurrentThread.Priority = ThreadPriority.Highest; // Elevate program's thread and process priority
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.High; // Note: NEVER set to RealTime

            // The program should always be closed by pressing Ctrl + C
            // to ensure the saving of any data that wasn't saved to redis

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            Console.CancelKeyPress += Close;

            var config = GameServerConfig.Config;
            using (var timer = new EasyTimer(LogLevel.Info, "Starting server...", $"Listening on port {config.Port} ([TIME])"))
            {
                EnumUtils.Load(); // Initialize and prepare our static classes for later use
                XmlLibrary.Load(config.XmlsDir);
                WorldLibrary.Load(config.WorldsDir);
                BehaviorLibrary.Load(config.BehaviorsDir);

                DbClient.Connect(DatabaseConfig.Config);

                ServerControl.Connect(MemberType.AppEngine, "GameServer",
                    new ServerInfo()
                    {
                        Port = config.Port,
                        Address = config.Address,
                        MaxPlayers = config.MaxPlayers,
                        AdminOnly = config.AdminOnly,
                    });

                RealmManager.Init(); // Finish setting up the game logic

                SocketServer.Start(config.Port, config.MaxPlayers); // Start the socket server to accept and manage TCP connections
            }

            RealmManager.Run(config.MsPT); // RealmManager runs world, entities and other game logic
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Log.Fatal(args.ExceptionObject);

            ServerControl.Publish(ControlChannel.MemberLeave, ServerControl.Host.InstanceID, null, ServerControl.Host);
        }

        private static void Close(object sender, ConsoleCancelEventArgs args)
        {
            ServerControl.Publish(ControlChannel.MemberLeave, ServerControl.Host.InstanceID, null, ServerControl.Host);
        }
    }
}
