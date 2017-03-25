using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Rocket.API.Commands;
using Rocket.API.Exceptions;
using Rocket.API.Player;
using Rocket.API.Providers;
using Rocket.API.Providers.Commands;
using Rocket.API.Providers.Plugins;
using Rocket.API.Providers.Remoting;

namespace Rocket.Core.Providers.Commands
{
    [RocketProviderProxy]
    public class RocketCommandProviderProxy : IRocketCommandProvider
    {
        public ReadOnlyCollection<IRocketCommand> Commands {
            get {
                List<IRocketCommand> result = new List<IRocketCommand>();
                foreach (IRocketCommandProvider provider in R.Providers.GetProviders<IRocketCommandProvider>()) {
                    result.AddRange(provider.Commands);
                }
                return result.AsReadOnly();
            }
        }

        public bool Execute(IRocketPlayer caller, string commandString)
        {
            R.Logger.Debug("EXECUTE:"+commandString);
            string name = "";
            string[] parameters = new string[0];
            try
            {
                commandString = commandString.TrimStart('/');
                string[] commandParts = Regex.Matches(commandString, @"[\""](.+?)[\""]|([^ ]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture).Cast<Match>().Select(m => m.Value.Trim('"').Trim()).ToArray();

                if (commandParts.Length != 0)
                {
                    name = commandParts[0];
                    parameters = commandParts.Skip(1).ToArray();
                }
                if (caller == null) caller = new ConsolePlayer();

                List<IRocketCommand> commands = new List<IRocketCommand>();

                R.Logger.Debug("NAME:"+name);

                //TODO: Figure a way to prioritise commands

                if (commands.Count > 0)
                {
                    IRocketCommand command = commands[0];

                    bool cancelCommand = false;
                        try
                        {
                            command.Execute(caller, parameters);
                            R.Logger.Debug("EXECUTED");
                            return true;
                        }
                        catch (NoPermissionsForCommandException ex)
                        {
                            R.Logger.Warn(ex);
                        }
                        catch (WrongUsageOfCommandException)
                        {
                            //
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                }else
                {
                    R.Logger.Info("Command not found");
                }
            }
            catch (Exception ex)
            {
                R.Logger.Error("An error occured while executing " + name + " [" + String.Join(", ", parameters) + "]", ex);
            }
            return false;
        }

        public void Load(bool isReload = false)
        {

        }

        public void Unload()
        {

        }
    }
}
