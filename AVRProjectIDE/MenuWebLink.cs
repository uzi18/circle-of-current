using System;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVRProjectIDE
{
    public class MenuWebLink
    {
        private List<MenuWebLink> children = new List<MenuWebLink>();

        private ToolStripMenuItem menuItem;
        public ToolStripMenuItem MenuItem
        {
            get { return menuItem; }
        }

        private string url;

        public MenuWebLink(string text, string url)
        {
            menuItem = new ToolStripMenuItem(text.Trim());
            url = url.Trim();

            if (string.IsNullOrEmpty(url) == false)
            {
                menuItem.ToolTipText = url;
                this.url = url;
                if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                {
                    menuItem.Image = global::AVRProjectIDE.Properties.Resources.web;
                }
                menuItem.Click += new EventHandler(menuItem_Click);
            }
        }

        public void Add(XmlElement xContainer)
        {
            foreach (XmlElement xEle in xContainer.ChildNodes)
            {
                MenuWebLink child = new MenuWebLink(xEle.GetAttribute("Text"), xEle.GetAttribute("URL"));

                children.Add(child);
                menuItem.DropDownItems.Add(child.MenuItem);

                child.Add(xEle);
            }
        }

        public void Add(string text, string url, bool clickable)
        {
            Add(new MenuWebLink(text, url));
        }

        public void Add(MenuWebLink mwl)
        {
            children.Add(mwl);
            menuItem.DropDownItems.Add(mwl.MenuItem);
        }

        private void OpenURL(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
            }
        }

        void menuItem_Click(object sender, EventArgs e)
        {
            OpenURL(url);
        }

        public static ToolStripMenuItem GetMenuLinkRoot(string text, string filepath)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            item.Image = global::AVRProjectIDE.Properties.Resources.web;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filepath);
            XmlElement xDocEle = xDoc.DocumentElement;

            foreach (XmlElement xEle in xDocEle.ChildNodes)
            {
                MenuWebLink link = new MenuWebLink(xEle.GetAttribute("Text"), xEle.GetAttribute("URL"));
                link.Add(xEle);

                item.DropDownItems.Add(link.MenuItem);
            }

            return item;
        }
    }
}
