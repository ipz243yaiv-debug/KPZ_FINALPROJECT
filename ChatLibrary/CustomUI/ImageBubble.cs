using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace ChatLibrary.CustomUI
{
    public class ImageBubble : Control
    {
        private Image _image;
        private bool _isMine;
        private int _bubbleWidth;
        private int _bubbleHeight;

        public ImageBubble(byte[] imageData, bool isMine)
        {
            this._isMine = isMine;
            this.DoubleBuffered = true;

            using (MemoryStream ms = new MemoryStream(imageData))
            {
                _image = Image.FromStream(ms);
            }

            CalculateSize();
        }

        private void CalculateSize()
        {
            int maxWidth = 200;
            int maxHeight = 200;

            int newWidth = _image.Width;
            int newHeight = _image.Height;

            if (_image.Width > maxWidth || _image.Height > maxHeight)
            {
                double ratioX = (double)maxWidth / _image.Width;
                double ratioY = (double)maxHeight / _image.Height;
                double ratio = Math.Min(ratioX, ratioY);

                newWidth = (int)(_image.Width * ratio);
                newHeight = (int)(_image.Height * ratio);
            }

            _bubbleWidth = newWidth + 20;
            _bubbleHeight = newHeight + 20;

            this.Height = _bubbleHeight;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int x = _isMine ? this.Width - _bubbleWidth - 10 : 10;
            int y = 5;

            Rectangle rect = new Rectangle(x, y, _bubbleWidth, _bubbleHeight - 10);

            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = 10;
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();

                Color bubbleColor = _isMine ? Color.FromArgb(220, 248, 198) : Color.White;
                using (SolidBrush brush = new SolidBrush(bubbleColor))
                {
                    g.FillPath(brush, path);
                }
            }

            Rectangle imgRect = new Rectangle(rect.X + 10, rect.Y + 5, _bubbleWidth - 20, _bubbleHeight - 20);
            g.DrawImage(_image, imgRect);
        }
    }
}