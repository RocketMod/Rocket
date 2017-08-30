using System;

namespace Rocket.API.Providers.Database
{
    public class ContextInitializationResult 
    {
        public ContextInitializationResult(ContextInitializationState state)
        {
            State = state;
        }

        public ContextInitializationState State { get; }
        public Exception Exception { get; set; }
    }
}