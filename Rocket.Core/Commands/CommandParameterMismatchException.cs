namespace Rocket.Core.Commands {
    public class CommandParameterMismatchException : CommandWrongUsageException
    {
        public CommandParameterMismatchException() : base("Command has been called with wrong parameters") { }

        public CommandParameterMismatchException(string message) : base(message) { }
    }
}