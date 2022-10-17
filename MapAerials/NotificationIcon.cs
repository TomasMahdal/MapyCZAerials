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
        private MainViewModel parent;

        public NotificationIcon(MainViewModel mainViewModel)
        {
            parent = mainViewModel;

            notifyIcon = new NotifyIcon();

            // main properties of notifyIcon
            notifyIcon.Text = "MapAerials";
            notifyIcon.Icon = Properties.Resources.notifyIcon;
            notifyIcon.Visible = true;

            // click on icon
            notifyIcon.Click += parent.ShowMainForm;

            CreateContextMenu();
        }

        /// <summary>
        /// generate context menu for notifyIcon
        /// </summary>
        public void CreateContextMenu()
        {
            ContextMenuStrip cs = new ContextMenuStrip();
            cs.Items.Add(Languages.GetString("notifyIcon_exit"), null, parent.ExitApp);

            notifyIcon.ContextMenuStrip = cs;
        }
    }
}
