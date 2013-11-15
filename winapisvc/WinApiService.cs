using System.ServiceProcess;
using Socket.BinaryCommandProcessor;
using Sockets.Listener;

namespace winapisvc
{
    public partial class WinApiService : ServiceBase
    {
        private BinaryProtocolListener _list;

        public WinApiService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _list = new BinaryProtocolListener(10137, new BinaryCommandProcessor());
            _list.Start();
        }

        protected override void OnStop()
        {
            _list.Dispose();
        }
    }
}
