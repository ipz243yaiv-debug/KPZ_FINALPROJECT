using System;
using System.Drawing;
using System.Windows.Forms;
using Client.UI;
using ChatLibrary.Security;

namespace Client
{
    public class AuthForm : Form
    {
        private IUIFactory _uiFactory;
        private TextBox _txtLogin;
        private TextBox _txtPassword;
        private Button _btnSubmit;
        private Button _btnSwitch;
        private Label _lblTitle;
        private bool _isLoginMode = true;

        public AuthForm()
        {
            _uiFactory = new WinFormsUIFactory();
            InitializeCustomLayout();
        }

        private void InitializeCustomLayout()
        {
            this.Text = "Месенджер";
            this.Size = new Size(350, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.FormBorderStyle = FormBorderStyle.None;

            Panel header = _uiFactory.CreatePanel(0, 0, 350, 40, Color.FromArgb(45, 45, 48));
            _lblTitle = _uiFactory.CreateLabel("LOGIN", 140, 10);
            _lblTitle.ForeColor = Color.White;
            _lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            header.Controls.Add(_lblTitle);

            Label lblLog = _uiFactory.CreateLabel("Username:", 50, 80);
            lblLog.ForeColor = Color.Gray;
            _txtLogin = _uiFactory.CreateTextBox(50, 110, 250);

            Label lblPass = _uiFactory.CreateLabel("Password:", 50, 180);
            lblPass.ForeColor = Color.Gray;
            _txtPassword = _uiFactory.CreateTextBox(50, 210, 250);
            _txtPassword.PasswordChar = '●';

            _btnSubmit = _uiFactory.CreateButton("SIGN IN", 50, 280, 250, 45);
            _btnSubmit.BackColor = Color.FromArgb(0, 122, 204);
            _btnSubmit.ForeColor = Color.White;
            _btnSubmit.FlatStyle = FlatStyle.Flat;
            _btnSubmit.FlatAppearance.BorderSize = 0;
            _btnSubmit.Click += BtnSubmit_Click;

            _btnSwitch = _uiFactory.CreateButton("Don't have an account? Register", 50, 340, 250, 30);
            _btnSwitch.BackColor = Color.Transparent;
            _btnSwitch.ForeColor = Color.Gray;
            _btnSwitch.FlatStyle = FlatStyle.Flat;
            _btnSwitch.FlatAppearance.BorderSize = 0;
            _btnSwitch.Click += BtnSwitch_Click;

            Button btnClose = _uiFactory.CreateButton("EXIT", 50, 420, 250, 40);
            btnClose.BackColor = Color.FromArgb(60, 60, 60);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Application.Exit();

            this.Controls.Add(header);
            this.Controls.Add(lblLog);
            this.Controls.Add(_txtLogin);
            this.Controls.Add(lblPass);
            this.Controls.Add(_txtPassword);
            this.Controls.Add(_btnSubmit);
            this.Controls.Add(_btnSwitch);
            this.Controls.Add(btnClose);
        }

        private void BtnSwitch_Click(object sender, EventArgs e)
        {
            _isLoginMode = !_isLoginMode;
            _lblTitle.Text = _isLoginMode ? "LOGIN" : "REGISTRATION";
            _lblTitle.Left = _isLoginMode ? 140 : 110;
            _btnSubmit.Text = _isLoginMode ? "SIGN IN" : "CREATE ACCOUNT";
            _btnSwitch.Text = _isLoginMode ? "Don't have an account? Register" : "Already have an account? Login";
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            string user = _txtLogin.Text.Trim();
            string pass = _txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Заповніть усі поля!");
                return;
            }

            var connector = ChatServerConnector.GetInstance();
            string passHash = PasswordHasher.HashPassword(pass);

            try
            {
                if (_isLoginMode)
                {
                    int? userId = connector.Login(user, passHash);
                    if (userId.HasValue)
                    {
                        Form1 mainForm = new Form1(userId.Value, user);
                        mainForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Невірний логін або пароль!");
                    }
                }
                else
                {
                    bool success = connector.Register(user, passHash);
                    if (success)
                    {
                        MessageBox.Show("Реєстрація успішна! Тепер увійдіть.");
                        BtnSwitch_Click(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Користувач з таким ім'ям вже існує.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка зв'язку з сервером: {ex.Message}");
            }
        }
    }
}