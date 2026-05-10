using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ChatLibrary.CustomUI
{
    public class MessageBubble : Control
    {
        private string text;
        private bool isMine;
        private Color bubbleColor;
        private int bubbleWidth;

        public MessageBubble(string text, bool isMine)
        {
            this.text = text;
            this.isMine = isMine;
            this.DoubleBuffered = true;
            this.Font = new Font("Segoe UI", 10);

            if (this.isMine)
                bubbleColor = Color.FromArgb(220, 248, 198);
            else
                bubbleColor = Color.White;

            SetSize();
        }

        private void SetSize()
        {
            using (Graphics g = this.CreateGraphics())
            {
                SizeF size = g.MeasureString(text + " ", this.Font, 250);

                bubbleWidth = (int)Math.Ceiling(size.Width) + 20;
                this.Height = (int)Math.Ceiling(size.Height) + 20;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int x = isMine ? this.Width - bubbleWidth - 10 : 10;
            int y = 5;

            Rectangle rect = new Rectangle(x, y, bubbleWidth, this.Height - 10);

            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = 10;
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();

                using (SolidBrush brush = new SolidBrush(bubbleColor))
                {
                    g.FillPath(brush, path);
                }
            }

            RectangleF textRect = new RectangleF(rect.X + 10, rect.Y + 5, rect.Width - 10, rect.Height - 10);

            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                g.DrawString(text, this.Font, textBrush, textRect);
            }
        }
    }
}