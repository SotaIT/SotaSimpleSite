using System.Drawing;
using System.Drawing.Text;

namespace Sota.Web.UI.WebControls
{
    internal class CodeImageInfo
    {
        public CodeImageInfo()
        {

        }

        public Font Font        = new Font("Arial", 12, FontStyle.Bold);
        public Size Size        = new Size(100, 40);
        public string ImageUrl  = string.Empty;
        public Color ForeColor  = Color.Black;
        public string Code      = string.Empty;
        public int Angle        = 15;
        public TextRenderingHint TextRenderingHint = TextRenderingHint.SystemDefault;
    }
}
