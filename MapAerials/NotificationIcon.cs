using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapAerials
{
    class NotificationIcon
    {
        private NotifyIcon notifyIcon;

        public NotificationIcon()
        {
            notifyIcon = new NotifyIcon();

            notifyIcon.Text = "MapAerials";
            notifyIcon.Icon = Properties.Resources.notifyIcon;
            notifyIcon.Visible = true;
        }
    }
}
