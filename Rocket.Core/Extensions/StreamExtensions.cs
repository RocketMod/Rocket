using System.IO;
using System.Text;

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
            StreamWriter writer = new StreamWriter(stream, encoding);
            writer.Write(s);
            writer.Flush();
        }

        public static string ConvertToString(this Stream stream) => stream.ConvertToString(Encoding.UTF8);

        public static string ConvertToString(this Stream stream, Encoding encoding)
        {
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            return encoding.GetString(ms.ToArray());
        }
    }
}