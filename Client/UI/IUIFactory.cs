using System.Drawing;
using System.Windows.Forms;

namespace Client.UI
{
    public interface IUIFactory
    {
        Button CreateButton(string text, int x, int y, int width, int height);
        TextBox CreateTextBox(int x, int y, int width);
        Label CreateLabel(string text, int x, int y);
        Panel CreatePanel(int x, int y, int width, int height, Color backColor);
    }
}
