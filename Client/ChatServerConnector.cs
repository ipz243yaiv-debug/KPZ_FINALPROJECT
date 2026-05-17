using Client.ChatServiceReference;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ServiceModel;

namespace Client
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class ChatServerConnector : IChatServiceCallback
    {
        private static readonly Lazy<ChatServerConnector> _instance =
    new Lazy<ChatServerConnector>(() => new ChatServerConnector());
        private ChatServiceClient Client;

        public event Action<string> OnMessageReceived;
        public event Action<Dictionary<int, string>> OnUsersUpdated;
        public event Action<string, byte[], string> OnFileReceived;

        private ChatServerConnector()
        {
            InstanceContext context = new InstanceContext(this);
            Client = new ChatServiceClient(context);
        }

        public static ChatServerConnector GetInstance()
            {
            return _instance.Value;
            }

        public int? Login(string username, string password)
        {
            return Client.Login(username, password);
        }

        public bool Register(string username, string password)
        {
            return Client.Register(username, password);
        }

        public bool UpdateProfile(int userId, string newNickname)
        {
            return Client.UpdateProfile(userId, newNickname);
        }

        public void Connect(int userId, string username)
        {
            if (Client.State == CommunicationState.Opened)
            {
                Client.Connect(userId, username);
            }
        }

        public void SendMessageToServer(string message, int senderId, int? targetId)
        {
            if (Client.State == CommunicationState.Opened)
            {
                Client.SendMessage(message, senderId, targetId);
            }
        }

        public void SendFileToServer(string fileName, byte[] fileData, int senderId, int? targetId)
        {
            if (Client.State == CommunicationState.Opened)
            {
                Client.SendFile(fileName, fileData, senderId, targetId);
            }
        }

        public void SendMessageToClient(string message)
        {
            OnMessageReceived?.Invoke(message);
        }

        public void UpdateUsersList(Dictionary<int, string> users)
        {
            OnUsersUpdated?.Invoke(users);
        }

        public void ReceiveFile(string fileName, byte[] fileData, string senderName)
        {
            OnFileReceived?.Invoke(fileName, fileData, senderName);
        }

        public void Disconnect(int id)
        {
            if (Client.State == CommunicationState.Opened)
            {
                Client.Disconnect(id);
            }
        }
    }
}
