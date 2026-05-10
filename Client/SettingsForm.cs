using System;
using System.Drawing;
using System.Windows.Forms;
using Client.UI;

namespace Client
{
    public class SettingsForm : Form
    {
        private IUIFactory _uiFactory;

        public SettingsForm()
        {
            _uiFactory = new WinFormsUIFactory();
            InitializeProgrammaticUI();
        }

        private void InitializeProgrammaticUI()
        {
            this.Text = "Налаштування месенджера";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;


            Label lblTheme = _uiFactory.CreateLabel("Вибір теми чату:", 20, 20);

            Button btnDarkTheme = _uiFactory.CreateButton("Темна тема", 20, 50, 150, 40);
            btnDarkTheme.Click += BtnDarkTheme_Click;

            Button btnLightTheme = _uiFactory.CreateButton("Світла тема", 190, 50, 150, 40);
            btnLightTheme.Click += BtnLightTheme_Click;

            Label lblFont = _uiFactory.CreateLabel("Розмір шрифту (в розробці):", 20, 110);
            TextBox txtFontSize = _uiFactory.CreateTextBox(20, 140, 150);
            txtFontSize.Text = "10";
            txtFontSize.Enabled = false; 

            Button btnSave = _uiFactory.CreateButton("Закрити", 120, 200, 150, 40);
            btnSave.BackColor = Color.LightGreen;
            btnSave.Click += (s, e) => { this.Close(); };

            this.Controls.Add(lblTheme);
            this.Controls.Add(btnDarkTheme);
            this.Controls.Add(btnLightTheme);
            this.Controls.Add(lblFont);
            this.Controls.Add(txtFontSize);
            this.Controls.Add(btnSave);
        }

        private void BtnDarkTheme_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(45, 45, 48);
            MessageBox.Show("Темна тема застосована до вікна налаштувань!");
        }

        private void BtnLightTheme_Click(object sender, EventArgs e)
        {
            this.BackColor = SystemColors.Control;
            MessageBox.Show("Світла тема застосована до вікна налаштувань!");
        }
    }
}