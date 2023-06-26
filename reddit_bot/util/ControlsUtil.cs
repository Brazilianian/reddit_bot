using System.Drawing;
using System.Windows.Forms;
using System;

namespace reddit_bor.util
{
    public static class ControlsUtil
    {
        public static void UnselectTask(object sender, EventArgs e)
        {
            ((Control)sender).BackColor = SystemColors.Control;
        }

        public static void SelectTask(object sender, EventArgs e)
        {
            ((Control)sender).BackColor = SystemColors.ActiveCaption;
        }
    }
}
