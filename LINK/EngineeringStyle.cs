using System.Drawing;
using System.Windows.Forms;

namespace LINK
{
    internal static class EngineeringStyle
    {
        public static Color BackgroundColor { get; } = Color.FromArgb(240, 242, 245);

        public static void ApplyModernStyle(Form form)
        {
            if (form == null) return;
            form.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            form.BackColor = BackgroundColor;
        }

        public static void StyleSidebarButton(Button button, bool isActive)
        {
            if (button == null) return;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = isActive ? Color.FromArgb(200, 210, 230) : Color.White;
            button.ForeColor = isActive ? Color.DarkBlue : Color.Black;
            button.Font = new Font("Segoe UI", 11F, isActive ? FontStyle.Bold : FontStyle.Regular);
        }

        public static void StyleInputControl(Control control)
        {
            if (control == null) return;
            control.BackColor = Color.White;
            control.ForeColor = Color.Black;
            control.Font = new Font("Segoe UI", 12F, control.Font.Style, GraphicsUnit.Point);
            if (control is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
            if (control is Label label)
            {
                label.AutoSize = false;
                label.TextAlign = ContentAlignment.MiddleRight;
            }
        }
    }
}
