using System.Threading.Tasks;

namespace XOPE_UI.Spy
{
    public interface IMessageReceiver
    {
        bool IsConnected { get; }

        // Not sure where to put this paramater
        Task StartReceiver(string receiverName);
        void ShutdownAndWait();
    }
}
