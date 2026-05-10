using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatLibrary.CustomUI
{
    public class CustomTextBox : TextBox
    {
        private Color _borderColor = Color.FromArgb(200, 200, 200);
        private Color _focusedBorderColor = Color.FromArgb(0, 122, 204);

        public CustomTextBox()
        {
            this.BorderStyle = BorderStyle.None;
            this.Font = new Font("Segoe UI", 11);
            this.Padding = new Padding(10);
        }

        protected override void OnEnter(EventArgs e) { _borderColor = _focusedBorderColor; Invalidate(); base.OnEnter(e); }
        protected override void OnLeave(EventArgs e) { _borderColor = Color.FromArgb(200, 200, 200); Invalidate(); base.OnLeave(e); }

    }
}