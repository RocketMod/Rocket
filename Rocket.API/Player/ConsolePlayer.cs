namespace Rocket.API.Player
{
    public class ConsolePlayer : IRocketPlayer
    {
        public string Id
        {
            get
            {
                return "Console";
            }
        }

        public string DisplayName
        {
            get
            {
                return "Console";
            }
        }

        public bool IsAdmin
        {
            get
            {
                return true;
            }
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(obj);
        }

        public void Kick(string message)
        {
            //
        }

        public void Ban(string message, uint duration = 0)
        {
            //
        }
    }
}