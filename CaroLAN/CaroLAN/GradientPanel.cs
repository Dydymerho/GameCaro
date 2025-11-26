using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CaroLAN
{
    /// <summary>
    /// Panel v?i hi?u ?ng gradient cho giao di?n gaming
    /// </summary>
    public class GradientPanel : Panel
    {
        public Color ColorTop { get; set; } = Color.FromArgb(135, 206, 250);
        public Color ColorBottom { get; set; } = Color.FromArgb(100, 149, 237);

        protected override void OnPaint(PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                ColorTop,
                ColorBottom,
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
            base.OnPaint(e);
        }
    }
}
