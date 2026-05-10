using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatLibrary.CustomUI
{
    public class ChatHistoryPanel : FlowLayoutPanel
    {
        public ChatHistoryPanel()
        {
            this.AutoScroll = true;
            this.BackColor = Color.FromArgb(235, 235, 235);
            this.DoubleBuffered = true;
            this.FlowDirection = FlowDirection.TopDown;
            this.WrapContents = false;
        }

        public void AddMessage(string text, bool isMine)
        {
            text = text.Trim();
            MessageBubble bubble = new MessageBubble(text, isMine);
            bubble.Width = this.ClientSize.Width - 25;
            bubble.Margin = new Padding(0, 5, 0, 5);
            this.Controls.Add(bubble);
            this.ScrollControlIntoView(bubble);
        }

        public void AddImage(byte[] imageData, bool isMine)
        {
            ImageBubble bubble = new ImageBubble(imageData, isMine);
            bubble.Width = this.ClientSize.Width - 25;
            bubble.Margin = new Padding(0, 5, 0, 5);
            this.Controls.Add(bubble);
            this.ScrollControlIntoView(bubble);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (Control c in this.Controls)
            {
                c.Width = this.ClientSize.Width - 25;
            }
        }
    }
}