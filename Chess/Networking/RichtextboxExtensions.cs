using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
//using System.Windows.Forms.VisualStyles;
using System.Drawing;

namespace Chess.Networking
{
    public static class RichTextBoxExtensions
    {
        //Allows setting the colour for appends
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        //Allows setting of font and colour for appends
        public static void AppendText(this RichTextBox box, string text, Color color, Font font)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.SelectionFont = font;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}
