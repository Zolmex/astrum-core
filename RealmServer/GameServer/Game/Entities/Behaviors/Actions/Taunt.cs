using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class TauntInfo
    {
        public int CoolDownLeft;
    }

    public class Taunt : IBehaviorAction
    {
        private static readonly Random _rand = new Random();

        private readonly string[] _text;
        private readonly int _coolDownMS;
        private readonly float _probability;

        public Taunt(XElement xml)
        {
            _text = xml.GetAttribute<string>("text").Split("||");
            _coolDownMS = xml.GetAttribute<int>("coolDownMS", 0);
            _probability = xml.GetAttribute<float>("probability", 1f);
        }

        public Taunt(string text, int coolDownMS, float probability = 1f)
        {
            _text = text.Split("||");
            _coolDownMS = coolDownMS;
            _probability = probability;
        }

        public void Enter(Character host, BehaviorController controller)
        {
            controller.InsertResource(this, new TauntInfo());

            if (_coolDownMS == 0 && _rand.NextDouble() < _probability)
                host.World.BroadcastAll(plr =>
                {
                    if (host.DistSqr(plr) < Player.SIGHT_RADIUS_SQR)
                        plr.SendEnemy(host, _text.RandomElement());
                });
        }

        public void Tick(Character host, RealmTime time, BehaviorController controller)
        {
            if (_coolDownMS == 0)
                return;

            var resource = (TauntInfo)controller.GetResource(this);
            if (resource.CoolDownLeft > 0)
            {
                resource.CoolDownLeft -= time.ElapsedMsDelta;
                return;
            }

            resource.CoolDownLeft = _coolDownMS;
            if (_rand.NextDouble() < _probability)
                host.World.BroadcastAll(plr =>
                {
                    if (host.DistSqr(plr) < Player.SIGHT_RADIUS_SQR)
                        plr.SendEnemy(host, _text.RandomElement());
                });
        }

        public void Exit(Character host, RealmTime time, BehaviorController controller)
        {
            // Empty
        }
    }
}
