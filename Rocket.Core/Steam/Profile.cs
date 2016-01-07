using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Xml;

namespace Rocket.Core.Steam
{
    public class Profile
    {
        public ulong SteamID64 { get; set; }
        public string SteamID { get; set; }
        public string OnlineState { get; set; }
        public string StateMessage { get; set; }
        public string PrivacyState { get; set; }
        public ushort? VisibilityState { get; set; }
        public Uri AvatarIcon { get; set; }
        public Uri AvatarMedium { get; set; }
        public Uri AvatarFull { get; set; }
        public bool? IsVacBanned { get; set; }
        public string TradeBanState { get; set; }
        public bool? IsLimitedAccount { get; set; }
        public string CustomURL { get; set; }
        public DateTime? MemberSince { get; set; }
        public double? HoursPlayedLastTwoWeeks { get; set; }
        public string Headline { get; set; }
        public string Location { get; set; }
        public string RealName { get; set; }
        public string Summary { get; set; }
        public List<MostPlayedGame> MostPlayedGames { get; set; }
        public List<Group> Groups { get; set; }

        public class MostPlayedGame
        {
            public string Name { get; set; }
            public Uri Link { get; set; }
            public Uri Icon { get; set; }
            public Uri Logo { get; set; }
            public Uri LogoSmall { get; set; }
            public double? HoursPlayed { get; set; }
            public double? HoursOnRecord { get; set; }
        }

        public class Group
        {
            public ulong SteamID64 { get; set; }
            public bool IsPrimary { get; set; }
            public string Name { get; set; }
            public string URL { get; set; }
            public Uri AvatarIcon { get; set; }
            public Uri AvatarMedium { get; set; }
            public Uri AvatarFull { get; set; }
            public string Headline { get; set; }
            public string Summary { get; set; }
            public uint? MemberCount { get; set; }
            public uint? MembersInGame { get; set; }
            public uint? MembersInChat { get; set; }
            public uint? MembersOnline { get; set; }
        }

        public Profile(ulong steamID64)
        {
            SteamID64 = steamID64;
            Reload();
        }


        public void Reload()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(new WebClient().DownloadString("http://steamcommunity.com/profiles/" + SteamID64 + "?xml=1"));

            SteamID = doc["profile"]["steamID"].ParseString();
            OnlineState = doc["profile"]["onlineState"].ParseString();
            StateMessage = doc["profile"]["stateMessage"].ParseString();
            PrivacyState = doc["profile"]["privacyState"].ParseString();
            VisibilityState = doc["profile"]["visibilityState"].ParseUInt16();
            AvatarIcon = doc["profile"]["avatarIcon"].ParseUri();
            AvatarMedium = doc["profile"]["avatarMedium"].ParseUri();
            AvatarFull = doc["profile"]["avatarFull"].ParseUri();
            IsVacBanned = doc["profile"]["vacBanned"].ParseBool();
            TradeBanState = doc["profile"]["tradeBanState"].ParseString();
            IsLimitedAccount = doc["profile"]["isLimitedAccount"].ParseBool();

            CustomURL = doc["profile"]["customURL"]?.ParseString();
            MemberSince = doc["profile"]["memberSince"]?.ParseDateTime(new CultureInfo("en-US", false));
            HoursPlayedLastTwoWeeks = doc["profile"]["hoursPlayed2Wk"]?.ParseDouble();
            Headline = doc["profile"]["headline"]?.ParseString();
            Location = doc["profile"]["location"]?.ParseString();
            RealName = doc["profile"]["realname"]?.ParseString();
            Summary = doc["profile"]["summary"]?.ParseString();

            if (doc["profile"]["mostPlayedGames"] != null)
            {
                MostPlayedGames = new List<MostPlayedGame>();
                foreach (XmlElement mostPlayedGame in doc["profile"]["mostPlayedGames"].ChildNodes)
                {
                    MostPlayedGame newMostPlayedGame = new MostPlayedGame();
                    newMostPlayedGame.Name = mostPlayedGame["gameName"].ParseString();
                    newMostPlayedGame.Link = mostPlayedGame["gameLink"].ParseUri();
                    newMostPlayedGame.Icon = mostPlayedGame["gameIcon"].ParseUri();
                    newMostPlayedGame.Logo = mostPlayedGame["gameLogo"].ParseUri();
                    newMostPlayedGame.LogoSmall = mostPlayedGame["gameLogoSmall"].ParseUri();
                    newMostPlayedGame.HoursPlayed = mostPlayedGame["hoursPlayed"].ParseDouble();
                    newMostPlayedGame.HoursOnRecord = mostPlayedGame["hoursOnRecord"].ParseDouble();
                    MostPlayedGames.Add(newMostPlayedGame);
                }
            }

            if (doc["profile"]["groups"] != null)
            {
                Groups = new List<Group>();
                foreach (XmlElement group in doc["profile"]["groups"].ChildNodes)
                {
                    Group newGroup = new Group();
                    newGroup.IsPrimary = group.Attributes["isPrimary"].InnerText == "1";
                    newGroup.SteamID64 = ulong.Parse(group["groupID64"].InnerText);
                    newGroup.Name = group["groupName"].ParseString();
                    newGroup.URL = group["groupURL"].ParseString();
                    newGroup.Headline = group["headline"].ParseString();
                    newGroup.Summary = group["summary"].ParseString();
                    newGroup.AvatarIcon = group["avatarIcon"].ParseUri();
                    newGroup.AvatarMedium = group["avatarMedium"].ParseUri();
                    newGroup.AvatarFull = group["avatarFull"].ParseUri();
                    newGroup.MemberCount = group["memberCount"].ParseUInt32();
                    newGroup.MembersInChat = group["membersInChat"].ParseUInt32();
                    newGroup.MembersInGame = group["membersInGame"].ParseUInt32();
                    newGroup.MembersOnline = group["membersOnline"].ParseUInt32();
                    Groups.Add(newGroup);
                }
            }
        }
    }
    public static class XmlElementExtensions
    {
        public static string ParseString(this XmlElement element)
        {
            return element.InnerText;
        }


        public static DateTime? ParseDateTime(this XmlElement element, CultureInfo cultureInfo)
        {
            try
            {
                return element == null ? null : (DateTime?)DateTime.Parse(element.InnerText.Replace("st", "").Replace("nd", "").Replace("rd", "").Replace("th", ""), cultureInfo);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static double? ParseDouble(this XmlElement element)
        {
            try
            {
                return element == null ? null : (double?)double.Parse(element.InnerText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ushort? ParseUInt16(this XmlElement element)
        {
            try
            {
                return element == null ? null : (ushort?)ushort.Parse(element.InnerText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static uint? ParseUInt32(this XmlElement element)
        {
            try
            {
                return element == null ? null : (uint?)uint.Parse(element.InnerText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ulong? ParseUInt64(this XmlElement element)
        {
            try
            {
                return element == null ? null : (ulong?)ulong.Parse(element.InnerText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool? ParseBool(this XmlElement element)
        {
            try
            {
                return element == null ? null : (bool?)(element.InnerText == "1");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Uri ParseUri(this XmlElement element)
        {
            try
            {
                return element == null ? null : new Uri(element.InnerText);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}