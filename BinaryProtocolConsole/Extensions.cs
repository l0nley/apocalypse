using System.Collections.Generic;

namespace BinaryProtocolConsole
{
    public static class Extensions
    {
        public static string ToRemotePath(this string s, Dictionary<string, object> env)
        {
            return env["remoteDir"].ToString().EndsWith("\\")
                       ? env["remoteDir"] + s
                       : env["remoteDir"] + "\\" + s;
        }

        public static string ToLocalPath(this string s, Dictionary<string, object> env)
        {
            return env["localDir"].ToString().EndsWith("\\")
                       ? env["localDir"] + s
                       : env["localDir"] + "\\" + s;
        }
    }
}