using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core.Commands
{
    public static class CommandExtensions
    {
        public static IRocketPlayer GetRocketPlayerParameter(this string[] array, int index)
        {
            if (array.Length > index)
            {
                ulong id = 0;
                if (ulong.TryParse(array[index], out id) && id > 76561197960265728)
                {
                    return new RocketPlayerBase(id.ToString());
                }
            }
            return R.Implementation.GetAllPlayers().Where(p => p.DisplayName.Contains(array[index].ToString())).FirstOrDefault();
        }
    }
}
