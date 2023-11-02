﻿using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Logic
{
    public class UserCollection : SmartCollection<User>
    {
        private readonly Dictionary<int, User> _userAccIds = new Dictionary<int, User>();

        public override void Remove(int itemId)
        {
            _drops.Enqueue(itemId);

            lock (_dict)
                if (_dict.TryGetValue(itemId, out var user) && user.Account != null)
                    _userAccIds.Remove(user.Account.AccountId);
        }

        public void SetAccId(User user, int accId)
        {
            _userAccIds.TryAdd(accId, user);
        }

        public User Get(int accId)
        {
            lock (_dict)
            {
                if (!_userAccIds.TryGetValue(accId, out var ret))
                    return default;
                return ret;
            }
        }
    }
}
