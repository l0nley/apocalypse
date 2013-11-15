﻿using System;
using System.Reactive.Disposables;
using ReactiveSockets;
using Sockets.Definitions;

namespace Sockets.Listener
{
    public class BinaryProtocolListener : IDisposable
    {
        private readonly IBinaryCommandProcessor _processor;
        private readonly ReactiveListener _listener;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public BinaryProtocolListener(int port,IBinaryCommandProcessor processor)
        {
            _processor = processor;
            _listener = new ReactiveListener(port);
        }

        public void Start()
        {
            var sub  = _listener.Connections.Subscribe(next =>
                {
                    var proto = new BinaryProtoChannel(next);
                    var sockSub = proto.Receiver.Subscribe(sock => ProcessMessage(sock,proto));
                    _disposable.Add(sockSub);
                });
            _listener.Start();
            _disposable.Add(sub);
        }

        private void ProcessMessage(BinaryMessage sock, BinaryProtoChannel proto)
        {
            _processor.ProcessCommand(sock, proto);
        }

        public void Dispose()
        {
            _listener.Dispose();
            _disposable.Dispose();
        }
    }
}
