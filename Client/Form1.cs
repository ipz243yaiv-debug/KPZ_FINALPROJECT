using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Client.UI;
using Client.History;
using ChatLibrary.CustomUI;

namespace Client
{
    public partial class Form1 : Form
    {
        private Dictionary<int, string> _onlineUsers = new Dictionary<int, string>();
        private int? selectedTargetId = null;
        private int _myId;
        private HistoryManager _historyManager;
        private string _currentUsername;
        private CustomButton _btnAttach;
        private Dictionary<string, ChatHistoryPanel> _chatPanels = new Dictionary<string, ChatHistoryPanel>();
        private List<string> _activeChats = new List<string>() { "Усі (Груповий чат)" };
        private string _currentChatKey = "Group";

        public Form1(int id, string user)
        {
            InitializeComponent();
            _myId = id;
            _currentUsername = user;
            _historyManager = new HistoryManager(new TextFileStorage());

            ApplyModernTheme();
            InitializeChatLogic();
            AddSettingsButton();
            AddProfileButton();
            LoadLocalHistory();

            ConnectToServer();
        }

        private void ApplyModernTheme()
        {
            this.Text = $"Чат - {_currentUsername}";
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(600, 400);

            if (listBox1 != null) listBox1.Visible = false;
            if (textBox1 != null) textBox1.Visible = false;
            if (button1 != null) button1.Visible = false;

            _btnAttach = new CustomButton();
            _btnAttach.Text = "📎";
            _btnAttach.Size = new Size(40, 30);
            _btnAttach.Location = new Point(12, 440);
            _btnAttach.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _btnAttach.BackColor = Color.FromArgb(200, 200, 200);
            _btnAttach.ForeColor = Color.Black;
            _btnAttach.Click += BtnAttach_Click;
            this.Controls.Add(_btnAttach);

            textBox2.Location = new Point(57, 440);
            textBox2.Size = new Size(395, 30);
            textBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox2.BorderStyle = BorderStyle.FixedSingle;
            textBox2.Font = new Font("Segoe UI", 11);

            button2.Location = new Point(462, 440);
            button2.Size = new Size(100, 30);
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button2.FlatStyle = FlatStyle.Flat;
            button2.BackColor = Color.FromArgb(0, 122, 204);
            button2.ForeColor = Color.White;
            button2.FlatAppearance.BorderSize = 0;
            button2.Font = new Font("Segoe UI", 10);

            if (listBox2 != null) listBox2.Visible = false;

            ModernUserList modernUsers = new ModernUserList();
            modernUsers.Name = "listBox2";
            modernUsers.Location = new Point(580, 50);
            modernUsers.Size = new Size(190, 420);
            modernUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            modernUsers.SelectedIndexChanged += listBox2_SelectedIndexChanged;
            this.Controls.Add(modernUsers);
            listBox2 = modernUsers;

            SwitchToChat("Group");
        }

        private ChatHistoryPanel GetChatPanel(string chatKey)
        {
            if (!_chatPanels.ContainsKey(chatKey))
            {
                ChatHistoryPanel panel = new ChatHistoryPanel();
                panel.Location = new Point(12, 50);
                panel.Size = new Size(550, 380);
                panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                panel.Visible = false;
                this.Controls.Add(panel);
                _chatPanels[chatKey] = panel;
            }
            return _chatPanels[chatKey];
        }

        private void SwitchToChat(string chatKey)
        {
            _currentChatKey = chatKey;

            foreach (var panel in _chatPanels.Values)
            {
                panel.Visible = false;
            }

            ChatHistoryPanel activePanel = GetChatPanel(chatKey);
            activePanel.Visible = true;
            activePanel.BringToFront();
        }

        private void InitializeChatLogic()
        {
            var connector = ChatServerConnector.GetInstance();

            connector.OnMessageReceived += (msg) => {
                this.Invoke(new Action(() => AddMessage(msg, false)));
            };

            connector.OnUsersUpdated += (users) => {
                this.Invoke(new Action(() => UpdateOnlineList(users)));
            };

            connector.OnFileReceived += (fileName, data, senderName) => {
                this.Invoke(new Action(() => HandleIncomingFile(fileName, data, senderName)));
            };
        }

        private void ConnectToServer()
        {
            System.Threading.Tasks.Task.Run(() => {
                try
                {
                    ChatServerConnector.GetInstance().Connect(_myId, _currentUsername);
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() => MessageBox.Show($"Помилка підключення: {ex.Message}")));
                }
            });
        }

        private void AddSettingsButton()
        {
            IUIFactory factory = new WinFormsUIFactory();
            Button btnSettings = factory.CreateButton("Налаштування", 580, 10, 90, 32);
            btnSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSettings.Click += (s, e) =>
            {
                SettingsForm settings = new SettingsForm();
                settings.ShowDialog();
            };
            this.Controls.Add(btnSettings);
        }

        private void AddProfileButton()
        {
            IUIFactory factory = new WinFormsUIFactory();
            Button btnProfile = factory.CreateButton("Профіль", 680, 10, 90, 32);
            btnProfile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnProfile.Click += (s, e) =>
            {
                string newName = Microsoft.VisualBasic.Interaction.InputBox("Введіть новий нікнейм:", "Налаштування профілю", _currentUsername);
                if (!string.IsNullOrEmpty(newName) && newName != _currentUsername)
                {
                    if (ChatServerConnector.GetInstance().UpdateProfile(_myId, newName))
                    {
                        _currentUsername = newName;
                        this.Text = $"Чат - {newName}";
                        MessageBox.Show("Нікнейм успішно змінено!");
                    }
                    else
                    {
                        MessageBox.Show("Помилка: нікнейм вже зайнятий або сервер недоступний.");
                    }
                }
            };
            this.Controls.Add(btnProfile);
        }

        private void LoadLocalHistory()
        {
            var pastMessages = _historyManager.GetHistory();
            foreach (var msg in pastMessages)
            {
                AddMessage(msg, true);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string message = textBox2.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            if (selectedTargetId == -1)
            {
                MessageBox.Show("Користувач не в мережі.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ChatServerConnector.GetInstance().SendMessageToServer(message, _myId, selectedTargetId);
            textBox2.Clear();
        }

        private void BtnAttach_Click(object sender, EventArgs e)
        {
            if (selectedTargetId == -1)
            {
                MessageBox.Show("Користувач не в мережі.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Виберіть файл для відправки";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        byte[] fileData = File.ReadAllBytes(ofd.FileName);
                        string fileName = Path.GetFileName(ofd.FileName);

                        if (fileData.Length > 60000)
                        {
                            MessageBox.Show("Файл занадто великий (ліміт ~60 КБ).", "Помилка розміру", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        ChatServerConnector.GetInstance().SendFileToServer(fileName, fileData, _myId, selectedTargetId);

                        string ext = Path.GetExtension(fileName).ToLower();
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp")
                        {
                            string target = selectedTargetId == null ? "Group" : _onlineUsers[selectedTargetId.Value];
                            GetChatPanel(target).AddImage(fileData, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка: {ex.Message}");
                    }
                }
            }
        }

        private void HandleIncomingFile(string fileName, byte[] data, string senderName)
        {
            string targetChat = senderName;
            EnsureUserInList(targetChat);
            ChatHistoryPanel panel = GetChatPanel(targetChat);

            string ext = Path.GetExtension(fileName).ToLower();

            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp")
            {
                panel.AddImage(data, false);
            }
            else
            {
                DialogResult res = MessageBox.Show($"Користувач {senderName} надіслав вам файл '{fileName}'. Зберегти?", "Вхідний файл", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (res == DialogResult.Yes)
                {
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.FileName = fileName;
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllBytes(sfd.FileName, data);
                            MessageBox.Show("Збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        public void AddMessage(string message, bool isHistory)
        {
            string targetChat = "Group";
            bool isMine = false;
            string displayMessage = message;

            if (message.StartsWith("[Приватно від "))
            {
                int endName = message.IndexOf("]:");
                if (endName != -1)
                {
                    targetChat = message.Substring(14, endName - 14);
                    displayMessage = message.Substring(endName + 2).Trim();
                }
            }
            else if (message.StartsWith("[Ви для "))
            {
                int endName = message.IndexOf("]:");
                if (endName != -1)
                {
                    targetChat = message.Substring(8, endName - 8);
                    displayMessage = message.Substring(endName + 2).Trim();
                    isMine = true;
                }
            }
            else if (message.StartsWith("[Файл відправлено до "))
            {
                int endName = message.IndexOf("]:");
                if (endName != -1)
                {
                    targetChat = message.Substring(21, endName - 21);
                    displayMessage = message.Substring(endName + 2).Trim();
                    isMine = true;
                }
            }
            else if (message.StartsWith("[Файл від "))
            {
                int endName = message.IndexOf("]:");
                if (endName != -1)
                {
                    targetChat = message.Substring(10, endName - 10);
                    displayMessage = message.Substring(endName + 2).Trim();
                }
            }
            else if (message.StartsWith("[Файл надіслано в групу]:"))
            {
                isMine = true;
            }
            else
            {
                isMine = message.StartsWith(_currentUsername + ":");
            }

            EnsureUserInList(targetChat);
            ChatHistoryPanel panel = GetChatPanel(targetChat);
            panel.AddMessage(displayMessage, isMine);

            if (!isHistory)
            {
                _historyManager.Save(message);
            }
        }

        public void UpdateOnlineList(Dictionary<int, string> users)
        {
            _onlineUsers = users;
            foreach (var name in users.Values)
            {
                if (!_activeChats.Contains(name) && name != _currentUsername)
                {
                    _activeChats.Add(name);
                }
            }
            RefreshUserList();
        }

        private void EnsureUserInList(string name)
        {
            if (name == "Group" || name == "Усі (Груповий чат)") return;
            if (!_activeChats.Contains(name) && name != _currentUsername)
            {
                _activeChats.Add(name);
                this.Invoke(new Action(RefreshUserList));
            }
        }

        private void RefreshUserList()
        {
            string prevSelected = listBox2.SelectedItem?.ToString();
            listBox2.Items.Clear();

            foreach (var chat in _activeChats)
            {
                listBox2.Items.Add(chat);
            }

            if (prevSelected != null && listBox2.Items.Contains(prevSelected))
            {
                listBox2.SelectedItem = prevSelected;
            }
            else if (listBox2.Items.Count > 0 && listBox2.SelectedIndex == -1)
            {
                listBox2.SelectedIndex = 0;
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < 0) return;

            string selectedName = listBox2.SelectedItem.ToString();
            string chatKey = selectedName == "Усі (Груповий чат)" ? "Group" : selectedName;

            SwitchToChat(chatKey);

            if (chatKey == "Group")
            {
                selectedTargetId = null;
            }
            else
            {
                if (_onlineUsers.ContainsValue(selectedName))
                {
                    selectedTargetId = _onlineUsers.First(x => x.Value == selectedName).Key;
                }
                else
                {
                    selectedTargetId = -1;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                ChatServerConnector.GetInstance().Disconnect(_myId);
            }
            catch { }
            base.OnFormClosing(e);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}