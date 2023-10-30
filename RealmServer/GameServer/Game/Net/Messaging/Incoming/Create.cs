﻿using Common.Utilities.Net;
using Common.Utilities;
using GameServer.Game.Net.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Database;
using StackExchange.Redis;

namespace GameServer.Game.Net.Messaging.Incoming
{
    public class Create : IncomingPacket
    {
        public short ClassType { get; set; }
        public short SkinType { get; set; }

        public override PacketId ID => PacketId.CREATE;

        protected override void Read(NetworkReader rdr)
        {
            ClassType = rdr.ReadInt16();
            SkinType = rdr.ReadInt16();
        }

        protected override void Handle(User user)
        {
            var chr = DbClient.CreateCharacter(user.Account, ClassType, SkinType, out var result);
            if (chr == null)
            {
                user.SendFailure(Failure.DEFAULT, result.GetDescription());
                return;
            }
            else
            {
                var world = user.GameInfo.World;
                if (world == null || world.Deleted || !world.Alive)
                {
                    user.SendFailure(Failure.DEFAULT, "World does not exist.");
                    return;
                }
                user.Load(chr, world);
            }
        }

        public override string ToString()
        {
            var type = typeof(Create);
            var props = type.GetProperties();
            var ret = $"\n";
            foreach (var prop in props)
            {
                ret += $"{prop.Name}:{prop.GetValue(this)}";
                if (!(props.IndexOf(prop) == props.Length - 1))
                    ret += "\n";
            }
            return ret;
        }
    }
}