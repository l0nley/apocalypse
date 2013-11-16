using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveSockets;
using Sockets.Definitions;

namespace Sockets.Client
{
    public class BinaryProtocolClient :IDisposable
    {
        private readonly string _encryptionKey;
        private readonly ReactiveClient _client;

        private readonly CompositeDisposable _disposable;
        private readonly BinaryProtoChannel _proto;
        private readonly Dictionary<Guid,ManualResetEvent>  _stops = new Dictionary<Guid, ManualResetEvent>();
        private readonly Dictionary<Guid,BinaryMessage> _messages = new Dictionary<Guid, BinaryMessage>();

        public BinaryProtocolClient(string host,int port,string encryptionKey)
        {
            _encryptionKey = encryptionKey;
            _disposable = new CompositeDisposable();
            _client = new ReactiveClient(host, port);
            _disposable.Add(_client);
            _proto = new BinaryProtoChannel(_client,_encryptionKey);
            _proto.Receiver.Subscribe(SequnceCompleted);
            _disposable.Add(_proto);
        }

        private void SequnceCompleted(BinaryMessage next)
        {
            _messages.Add(next.SequenceId, next);
            var a = _stops[next.SequenceId];
            _stops.Remove(next.SequenceId);
            a.Set();
        }

        private async Task<BinaryMessage> AwaitResponse(BinaryMessage msg)
        {
            var eve = new ManualResetEvent(false);
            eve.Reset();
            _stops.Add(msg.SequenceId,eve);
            await _proto.SendAsync(msg);
            eve.WaitOne();
            var rmsg = _messages[msg.SequenceId];
            _messages.Remove(msg.SequenceId);
            return rmsg;
        }

        public void Connect()
        {
            _client.ConnectAsync().Wait();
        }

        private async Task<T> SendAndReceive<T>(OpCodes opCodes, object toSend,bool needParseWhenSuccess,Func<T,byte[],T> successConversion) where T : CommandResult
        {
            var guid = Guid.NewGuid();
            var msg = new BinaryMessage
            {
                SequenceId = guid,
                OpCode = (long)opCodes
            };
            if (toSend != null)
            {
                using (var ms = new MemoryStream())
                {
                    using (TextWriter tw = new StreamWriter(ms))
                    {
                        JsonSerializer.Create().Serialize(tw, toSend);
                    }
                    msg.Payload = ms.ToArray();
                }
            }
            var response = await AwaitResponse(msg);
            var res = Activator.CreateInstance<T>();
            res.OpCode = (OpCodes) response.OpCode;
            if (res.OpCode != OpCodes.Success)
            {
                return res;
            }
            if (needParseWhenSuccess)
            {
                res.JsonObject = JObject.Parse(Encoding.UTF8.GetString(response.Payload));
            }
            res = successConversion(res,response.Payload);
            return res;
        }

        public async Task<DirectoryListResult> ListDirectory(string path)
        {
            var obj = new
                {
                    fileName = path
                };
            return await SendAndReceive<DirectoryListResult>(OpCodes.ListDirectory, obj,true,(l,a) =>
                {
                    l.Directories =
                        l.JsonObject.GetValue("dirs").Children<JToken>().Select(m => m.Value<string>()).ToArray();
                    l.Files = l.JsonObject.GetValue("files").Children<JToken>().Select(m => m.Value<string>()).ToArray();
                    return l;
                });
        }


        public async Task<GetFileOptionsResult> GetFileOptions(string fileName)
        {
            var obj = new
                {
                    fileName
                };
            return await SendAndReceive<GetFileOptionsResult>(OpCodes.GetFileOptions, obj, true, (l,a) =>
                {
                    l.CreationTime = l.JsonObject.GetValue("creationTime").Value<DateTime>();
                    l.Exists = l.JsonObject.GetValue("exists").Value<bool>();
                    l.LastAccess = l.JsonObject.GetValue("lastAccess").Value<DateTime>();
                    l.LastWrite = l.JsonObject.GetValue("lastWrite").Value<DateTime>();
                    l.Size = l.JsonObject.GetValue("size").Value<long>();
                    return l;
                });
        }

        public async Task<GetFilePartResult> GetFilePart(string fileName, long offset, int count)
        {
            var obj = new
                {
                    fileName,
                    offset,
                    count
                };
            return await SendAndReceive<GetFilePartResult>(OpCodes.GetFilePart, obj, false, (l, a) =>
                {
                    l.Payload = a;
                    return l;
                });
        }

        public async Task<CommandResult> TouchFile(string fileName)
        {
            var obj = new
                {
                    fileName
                };
            return await SendAndReceive<CommandResult>(OpCodes.TouchFile, obj, false, (l, a) => l);
        }

        public async Task<CommandResult> SetFilePart(string fileName, long offset, byte[] bytes, bool reverseSeek = false)
        {
            var obj = new
                {
                    fileName,
                    offset,
                    bytes,
                    reverseSeek
                };
            return await SendAndReceive<CommandResult>(OpCodes.SetFilePart, obj, false, (l, a) => l);
        }


        public async Task<CommandResult> RunAndWait(string fileName, string args)
        {
            var obj = new
                {
                    fileName,
                    @params = args
                };
            return await SendAndReceive<CommandResult>(OpCodes.RunAndWait, obj, false, (l, a) => l);
        }

        public async Task<CommandResult> RunNoWait(string fileName, string args)
        {
            var obj = new
                {
                    fileName,
                    @params = args
                };
            return await SendAndReceive<CommandResult>(OpCodes.RunNoWait, obj, false, (l, a) => l);
        }

        public async Task<CommandResult> DropDirectory(string fileName, bool recursive)
        {
            var obj = new
                {
                    fileName,
                    recursive
                };
            return await SendAndReceive<CommandResult>(OpCodes.DropDir, obj, false, (l, a) => l);
        }

        public async Task<CommandResult> CreateDir(string fileName)
        {
            var obj = new
                {
                    fileName
                };
            return await SendAndReceive<CommandResult>(OpCodes.CreateDir, obj, false, (l, a) => l);
        }

        public async Task<CommandResult> DropFile(string fileName)
        {
            var obj = new
                {
                    fileName
                };
            return await SendAndReceive<CommandResult>(OpCodes.DropFile, obj, false, (l, a) => l);
        }


        public void Disconnect()
        {
            _client.Disconnect();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public class CommandResult
        {
            public OpCodes OpCode { get; set; }
            public JObject JsonObject { get; set; }
        }

        public class DirectoryListResult :CommandResult
        {
            public string[] Files { get; set; }
            public string[] Directories { get; set; }
        }

        public class GetFileOptionsResult :CommandResult
        {
            public string FullName { get; set; }
            public DateTime LastAccess  {get;set;}
            public DateTime LastWrite { get; set; }
            public long Size { get; set; }
            public bool Exists { get; set; }
            public string Extension { get; set; }
            public bool IsReadOnly { get; set; }
            public DateTime CreationTime { get; set; }
        }

       

        public class GetFilePartResult :CommandResult
        {
            public byte[] Payload { get; set; }
        }


    }
}
