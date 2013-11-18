using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<BinaryMessage> SendAndReceive(BinaryMessage message)
        {
            var eve = new ManualResetEvent(false);
            _stops.Add(message.SequenceId,eve);
            await _proto.SendAsync(message);
            eve.WaitOne();
            var retmessage = _messages[message.SequenceId];
            _messages.Remove(message.SequenceId);
            return retmessage;
        }

        private void SequnceCompleted(BinaryMessage next)
        {
            _messages.Add(next.SequenceId, next);
            var a = _stops[next.SequenceId];
            _stops.Remove(next.SequenceId);
            a.Set();
        }

        public void Connect()
        {
            _client.ConnectAsync().Wait();
        }

        public async Task ConnectAsync()
        {
            await _client.ConnectAsync();
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
