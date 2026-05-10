using ChatLibrary.DAL;
using Server.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace ChatLibrary
{
    [ServiceContract(CallbackContract = typeof(IChatServiceCallback))]
    public interface IChatService
    {
        [OperationContract]
        int? Login(string username, string password);

        [OperationContract]
        bool Register(string username, string password);

        [OperationContract]
        bool UpdateProfile(int userId, string newNickname);

        [OperationContract(IsOneWay = true)]
        void Connect(int userId, string username);

        [OperationContract(IsOneWay = true)]
        void Disconnect(int id);

        [OperationContract(IsOneWay = true)]
        void SendMessage(string message, int senderId, int? targetId);

        [OperationContract(IsOneWay = true)]
        void SendFile(string fileName, byte[] fileData, int senderId, int? targetId);
    }

    public interface IChatServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void SendMessageToClient(string message);

        [OperationContract(IsOneWay = true)]
        void UpdateUsersList(Dictionary<int, string> users);

        [OperationContract(IsOneWay = true)]
        void ReceiveFile(string fileName, byte[] fileData, string senderName);
    }

    public class ChatUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public OperationContext Context { get; set; }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class ChatService : IChatService
    {
        private List<ChatUser> usersList = new List<ChatUser>();
        private DbConnectionManager _dbManager;
        private UserRepository _userRepo;
        private MessageRepository _messageRepo;

        public ChatService()
        {
            var config = new DatabaseConfig { DatabaseName = "ChatDB.sqlite", TimeoutSeconds = 30, UseForeignKeys = true };
            _dbManager = new DbConnectionManager(config);
            _dbManager.InitializeDatabase();
            _userRepo = new UserRepository(_dbManager);
            _messageRepo = new MessageRepository(_dbManager);
        }

        public int? Login(string username, string password)
        {
            return _userRepo.Authenticate(username, password);
        }

        public bool Register(string username, string password)
        {
            return _userRepo.RegisterUser(username, password);
        }

        public bool UpdateProfile(int userId, string newNickname)
        {
            if (_userRepo.UpdateNickname(userId, newNickname))
            {
                var user = usersList.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    user.Name = newNickname;
                }
                NotifyUsersUpdated();
                return true;
            }
            return false;
        }

        public void Connect(int userId, string username)
        {
            usersList.RemoveAll(u => u.Id == userId);

            ChatUser user = new ChatUser()
            {
                Id = userId,
                Name = username,
                Context = OperationContext.Current
            };

            usersList.Add(user);
            NotifyUsersUpdated();
        }

        public void Disconnect(int id)
        {
            var user = usersList.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                usersList.Remove(user);
                NotifyUsersUpdated();
            }
        }

        public void SendMessage(string message, int senderId, int? targetId)
        {
            var sender = usersList.FirstOrDefault(u => u.Id == senderId);
            if (sender == null) return;

            _messageRepo.SaveMessage(senderId, targetId, message);

            if (targetId.HasValue)
            {
                var receiver = usersList.FirstOrDefault(u => u.Id == targetId.Value);
                if (receiver != null)
                {
                    try
                    {
                        receiver.Context.GetCallbackChannel<IChatServiceCallback>().SendMessageToClient($"[Приватно від {sender.Name}]: {message}");
                        sender.Context.GetCallbackChannel<IChatServiceCallback>().SendMessageToClient($"[Ви для {receiver.Name}]: {message}");
                    }
                    catch
                    {
                        HandleDisconnect(receiver);
                    }
                }
            }
            else
            {
                string broadcastMsg = $"{sender.Name}: {message}";
                foreach (var user in usersList.ToList())
                {
                    try
                    {
                        user.Context.GetCallbackChannel<IChatServiceCallback>().SendMessageToClient(broadcastMsg);
                    }
                    catch
                    {
                        HandleDisconnect(user);
                    }
                }
            }
        }

        public void SendFile(string fileName, byte[] fileData, int senderId, int? targetId)
        {
            var sender = usersList.FirstOrDefault(u => u.Id == senderId);
            if (sender == null) return;

            if (targetId.HasValue)
            {
                var receiver = usersList.FirstOrDefault(u => u.Id == targetId.Value);
                if (receiver != null)
                {
                    try
                    {
                        receiver.Context.GetCallbackChannel<IChatServiceCallback>().ReceiveFile(fileName, fileData, sender.Name);
                        receiver.Context.GetCallbackChannel<IChatServiceCallback>().SendMessageToClient($"[Файл від {sender.Name}]: {fileName}");
                        sender.Context.GetCallbackChannel<IChatServiceCallback>().SendMessageToClient($"[Файл відправлено до {receiver.Name}]: {fileName}");
                    }
                    catch { HandleDisconnect(receiver); }
                }
            }
            else
            {
                foreach (var user in usersList.ToList())
                {
                    if (user.Id == senderId) continue;
                    try
                    {
                        user.Context.GetCallbackChannel<IChatServiceCallback>().ReceiveFile(fileName, fileData, sender.Name);
                        user.Context.GetCallbackChannel<IChatServiceCallback>().SendMessageToClient($"[Файл від {sender.Name}]: {fileName}");
                    }
                    catch { HandleDisconnect(user); }
                }
                sender.Context.GetCallbackChannel<IChatServiceCallback>().SendMessageToClient($"[Файл надіслано в групу]: {fileName}");
            }
        }

        private void HandleDisconnect(ChatUser user)
        {
            if (usersList.Contains(user))
            {
                usersList.Remove(user);
                NotifyUsersUpdated();
            }
        }

        private void NotifyUsersUpdated()
        {
            var dict = usersList.ToDictionary(u => u.Id, u => u.Name);

            foreach (var user in usersList.ToList())
            {
                try
                {
                    user.Context.GetCallbackChannel<IChatServiceCallback>().UpdateUsersList(dict);
                }
                catch
                {
                    usersList.Remove(user);
                }
            }
        }
    }
}