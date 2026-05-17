using ChatLibrary.CustomUI; 
using System.Drawing;
using System.Windows.Forms;

namespace Client.UI
{
    public class WinFormsUIFactory : IUIFactory
    {
 
        public Button CreateButton(string text, int x, int y, int width, int height)
        {


            return new CustomButton
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };
        }

 
        public TextBox CreateTextBox(int x, int y, int width)
        {


            return new CustomTextBox
            {
                Location = new Point(x, y),
                Width = width,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 30, 30)
            };
        }


        public Label CreateLabel(string text, int x, int y)
        {


            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(45, 45, 45)
            };
        }


        public Panel CreatePanel(int x, int y, int width, int height, Color backColor)
        {


            return new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = backColor,
                BorderStyle = BorderStyle.None 
            };
        }
    }
}
