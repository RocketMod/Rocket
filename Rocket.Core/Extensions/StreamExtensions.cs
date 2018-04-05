using System.IO;
using System.Text;

#if NET35
using Rocket.Compability; //backport Stream.CopyTo(...)
#endif

namespace Rocket.Core.Extensions
{
    public static class StreamExtensions
    {
        public static void Write(this Stream stream, string s)
        {
            stream.Write(s, Encoding.UTF8);
        }

        public static void Write(this Stream stream, string s, Encoding encoding)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms, encoding);
            writer.Write(s);
            writer.Flush();
            ms.Position = 0;
            ms.CopyTo(stream);
        }

        public static string ConvertToString(this Stream stream)
        {
            return stream.ConvertToString(Encoding.UTF8);
        }

        public static string ConvertToString(this Stream stream, Encoding encoding)
        {
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            return encoding.GetString(ms.ToArray());
        }
    }
}