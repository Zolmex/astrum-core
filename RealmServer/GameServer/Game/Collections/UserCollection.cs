using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Collections
{
    public class UserCollection : SmartCollection<User>
    {
        private readonly Dictionary<int, User> _userAccIds = new Dictionary<int, User>();

        public override void Remove(int itemId)
        {
            _drops.Enqueue(itemId);

            lock (_dict)
                if (_dict.TryGetValue(itemId, out var user) && user.Account != null)
                    _userAccIds.Remove(user.Account.AccountId); // Remove the acc id here and not on update because Account may be null
        }

        public void SetAccId(User user, int accId)
        {
            _userAccIds.TryAdd(accId, user);
        }

        public User GetByAccId(int accId)
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
