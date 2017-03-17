using System;
using System.Runtime.Serialization;

namespace Rocket.API
{
    [Serializable]
    [DataContract]
    public class RocketPlayerBase : IRocketPlayer
    {
        [DataMember]
        public string Id { get; private set; }

        [DataMember]
        public string DisplayName { get; private set; }

        [DataMember]
        public bool IsAdmin { get; private set; }

        public RocketPlayerBase(string Id, string DisplayName = null, bool IsAdmin = false)
        {
            this.Id = Id;
            if (DisplayName == null)
            {
                DisplayName = Id;
            }
            else
            {
                this.DisplayName = DisplayName;
            }
            this.IsAdmin = IsAdmin;
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(((IRocketPlayer)obj).Id);
        }

        public void Kick(string message)
        {
            //
        }

        public void Ban(string message, uint duration)
        {
            //
        }
    }
}