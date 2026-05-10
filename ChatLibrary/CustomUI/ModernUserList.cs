using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ChatLibrary.CustomUI
{
    public class ModernUserList : ListBox
    {
        private int _hoveredIndex = -1;

        public ModernUserList()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.ItemHeight = 50; 
            this.BorderStyle = BorderStyle.None;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.DoubleBuffered = true;

            this.MouseMove += ModernUserList_MouseMove;
            this.MouseLeave += ModernUserList_MouseLeave;
        }

        private void ModernUserList_MouseMove(object sender, MouseEventArgs e)
        {
            int index = this.IndexFromPoint(e.Location);
            if (index != _hoveredIndex)
            {
                _hoveredIndex = index;
                this.Invalidate();
            }
        }

        private void ModernUserList_MouseLeave(object sender, EventArgs e)
        {
            _hoveredIndex = -1;
            this.Invalidate();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= this.Items.Count) return;

            string userName = this.Items[e.Index].ToString();
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            bool isHovered = e.Index == _hoveredIndex;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color backColor = this.BackColor;
            if (isSelected) backColor = Color.FromArgb(210, 230, 250); 
            else if (isHovered) backColor = Color.FromArgb(230, 230, 230); 

            using (SolidBrush bgBrush = new SolidBrush(backColor))
            {
                g.FillRectangle(bgBrush, e.Bounds);
            }

            Rectangle avatarRect = new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 10, 30, 30);

            int colorHash = Math.Abs(userName.GetHashCode()) % 200;
            Color avatarColor = Color.FromArgb(100 + (colorHash % 100), 100 + (colorHash / 2 % 100), 200);

            if (userName == "Усі (Груповий чат)") avatarColor = Color.FromArgb(0, 122, 204);

            using (SolidBrush avatarBrush = new SolidBrush(avatarColor))
            {
                g.FillEllipse(avatarBrush, avatarRect);
            }

            string initial = userName.Length > 0 ? userName.Substring(0, 1).ToUpper() : "?";
            if (userName.StartsWith("Усі")) initial = "👥";

            using (SolidBrush textBrush = new SolidBrush(Color.White))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(initial, new Font("Segoe UI", 12, FontStyle.Bold), textBrush, avatarRect, sf);
            }

            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                Rectangle textRect = new Rectangle(e.Bounds.X + 50, e.Bounds.Y, e.Bounds.Width - 50, e.Bounds.Height);
                StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(userName, new Font("Segoe UI", 11), textBrush, textRect, sf);
            }
        }
    }
}