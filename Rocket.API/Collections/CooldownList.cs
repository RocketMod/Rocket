using System;
using System.Collections.Generic;

namespace Rocket.API.Collections
{
    public delegate void CooldownExpired();
    public interface ICooldownItem
    {
        double Cooldown { get; }
        DateTime Started { get; set; }
        event CooldownExpired OnCooldownExpired;
        void Expire();
    }
    public class CooldownList<T> : Dictionary<string,T> where T : ICooldownItem
    {
        public new void Add(string key, T value)
        {
            value.Started = DateTime.UtcNow;
            base.Add(key, value);
        }
        public double CheckCooldown(string key)
        {
            T item;
            if (!TryGetValue(key, out item)) return 0;
            
            double timeSinceExecution = (DateTime.Now - item.Started).TotalSeconds;
            if (item.Cooldown <= timeSinceExecution)
            {
                item.Expire();
                Remove(key);
                return 0;
            }
            else
            {
                return item.Cooldown - (uint)timeSinceExecution;
            }
        }
    }
}
