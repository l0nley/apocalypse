using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sockets.Definitions;

namespace Socket.BinaryCommandProcessor
{
    public class BinaryCommandProcessor :IBinaryCommandProcessor
    {
        public void ProcessCommand(BinaryMessage sock, BinaryProtoChannel proto)
        {
            var response = new BinaryMessage {OpCode = (long) OpCodes.NotFound, SequenceId = sock.SequenceId, Payload = null};
            try
            {
                switch ((OpCodes) sock.OpCode)
                {
                    case OpCodes.ListDirectory:
                        ListDirectory(sock, response);
                        break;
                    case OpCodes.GetFileOptions:
                        GetFileOptions(sock, response);
                        break;
                    case OpCodes.GetFilePart:
                        GetFilePart(sock, response);
                        break;
                    case OpCodes.RunAndWait:
                        RunAndWait(sock);
                        break;
                    case OpCodes.RunNoWait:
                        RunNoWait(sock);
                        break;
                    case OpCodes.SetFilePart:
                        SetFilePart(sock);
                        break;
                    case OpCodes.DropFile:
                        DropFile(sock);
                        break;
                    case OpCodes.TouchFile:
                        TouchFile(sock);
                        break;
                        case OpCodes.CreateDir:
                        CreateDir(sock);
                        break;
                        case OpCodes.DropDir:
                        DropDirectory(sock);
                        break;
                }
                response.OpCode = (long) OpCodes.Success;
            }
            catch
            {
                response.OpCode = (long) OpCodes.Error;
            }
            finally
            {
                proto.SendAsync(response);
            }
        }

        private static void DropDirectory(BinaryMessage sock)
        {
            var obj = JObject.Parse(Encoding.UTF8.GetString(sock.Payload));
            var fileName = obj.GetValue("fileName").Value<string>();
            var recursive = obj.GetValue("recursive").Value<bool>();
            var di = new DirectoryInfo(fileName);
            di.Delete(recursive);
        }

        private static void DropFile(BinaryMessage sock)
        {
            var obj = JObject.Parse(Encoding.UTF8.GetString(sock.Payload));
            var fileName = obj.GetValue("fileName").Value<string>();
            File.Delete(fileName);
        }

        private static void CreateDir(BinaryMessage sock)
        {
            var obj = JObject.Parse(Encoding.UTF8.GetString(sock.Payload));
            var fileName = obj.GetValue("fileName").Value<string>();
            new DirectoryInfo(fileName).Create();
        }

        private static void TouchFile(BinaryMessage sock)
        {
            var obj = JObject.Parse(Encoding.UTF8.GetString(sock.Payload));
            var fileName = obj.GetValue("fileName").Value<string>();
            File.Create(fileName).Close();
        }

        private static void SetFilePart(BinaryMessage sock)
        {
            var obj = JObject.Parse(Encoding.UTF8.GetString(sock.Payload));
            var fileName = obj.GetValue("fileName").Value<string>();
            var offset = obj.GetValue("offset").Value<long>();
            var isOffsetBack = obj.GetValue("reverseSeek").Value<bool>();
            var bytes = (byte[]) obj.GetValue("bytes");
            using (var fs = File.Open(fileName, FileMode.Open))
            {
                using (var wr = new BinaryWriter(fs))
                {
                    wr.BaseStream.Seek(offset, isOffsetBack ? SeekOrigin.End : SeekOrigin.Begin);
                    wr.Write(bytes,0,bytes.Length);
                }
            }
        }

        private static void RunNoWait(BinaryMessage sock)
        {
            var obj = JObject.Parse(Encoding.UTF8.GetString(sock.Payload));
            var fileName = obj.GetValue("fileName").Value<string>();
            var @params = obj.GetValue("params").Value<string>();
            Process.Start(fileName, @params);
        }

        private static void RunAndWait(BinaryMessage sock)
        {
            var obj = JObject.Parse(Encoding.UTF8.GetString(sock.Payload));
            var fileName = obj.GetValue("fileName").Value<string>();
            var @params = obj.GetValue("params").Value<string>();
            // ReSharper disable PossibleNullReferenceException
            Process.Start(fileName, @params).WaitForExit();
            // ReSharper restore PossibleNullReferenceException
        }

        private static void GetFilePart(BinaryMessage sock, BinaryMessage response)
        {
            var obj = JObject.Parse(Encoding.UTF8.GetString(sock.Payload));
            var fileName = obj.GetValue("fileName").Value<string>();
            var offset = obj.GetValue("offset").Value<long>();
            var count = obj.GetValue("count").Value<int>();
            using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    br.BaseStream.Seek(offset, SeekOrigin.Begin);
                    var buff = new byte[count];
                    var readed = br.Read(buff, 0, buff.Length);
                    response.Payload = buff.Take(readed).ToArray();
                }
            }
        }

        private static void GetFileOptions(BinaryMessage sock, BinaryMessage response)
        {
            var obj = JObject.Parse(Encoding.UTF8.GetString(sock.Payload));
            var fileName = obj.GetValue("fileName").Value<string>();
            var fi = new FileInfo(fileName);
            var oo = new
                {
                    fullName = fi.FullName,
                    lastAccess = fi.LastAccessTimeUtc,
                    lastWrite = fi.LastWriteTimeUtc,
                    size = fi.Exists ? fi.Length : 0,
                    exists = fi.Exists,
                    extension = fi.Extension,
                    isReadOnly = fi.Exists && fi.IsReadOnly,
                    creationTime = fi.CreationTimeUtc
                };
            using (var ms = new MemoryStream())
            {
                using (TextWriter wr = new StreamWriter(ms))
                {
                    JsonSerializer.Create().Serialize(wr,oo);
                }
                response.Payload = ms.ToArray();
            }
        }

        private static void ListDirectory(BinaryMessage sock, BinaryMessage response)
        {
            var obj = JObject.Parse(Encoding.UTF8.GetString(sock.Payload));
            var fileName = obj.GetValue("fileName").Value<string>();
            var di = new DirectoryInfo(fileName);
            var o = new
                {
                    files = di.GetFiles().Select(l=>l.FullName).ToArray(),
                    dirs = di.GetDirectories().Select(l=>l.FullName).ToArray()
                };
            using (var ms = new MemoryStream())
            {
                using (TextWriter tw = new StreamWriter(ms))
                {
                    JsonSerializer.Create().Serialize(tw,o);
                }
                response.Payload = ms.ToArray();
            }
        }
    }
}