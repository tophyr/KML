﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KML
{
    class GuiTreeAttrib : ListViewItem
    {
        public KmlAttrib DataAttrib
        {
            get
            {
                return (KmlAttrib)DataContext;
            }
            private set
            {
                DataContext = value;
                Content = value;
            }
        }

        private static GuiIcons Icons = new GuiIcons16();

        public GuiTreeAttrib(KmlAttrib attrib)
        {
            DataAttrib = attrib;

            BuildContextMenu();
        }

        private void BuildContextMenu()
        {
            string shortHeader = DataAttrib.ToString();
            if (shortHeader.Length > 30)
            {
                shortHeader = shortHeader.Substring(0, 27) + "...";
            }

            ContextMenu menu = new ContextMenu();
            MenuItem title = new MenuItem();
            title.Header = shortHeader;
            title.IsEnabled = false;
            title.Background = new SolidColorBrush(Colors.Black);
            title.BorderThickness = new Thickness(1);
            title.BorderBrush = new SolidColorBrush(Colors.Gray);
            menu.Items.Add(title);
            menu.Items.Add(new Separator());

            // So far it's the default Menu, wich should not be shown if no items follow
            int defaultMenuCount = menu.Items.Count;

            MenuItem m = new MenuItem();
            m.DataContext = DataAttrib;
            m.Icon = Icons.CreateImage(Icons.Delete);
            m.Header = "Delete this attribute...";
            m.Click += AttribDelete_Click;
            m.IsEnabled = DataAttrib.CanBeDeleted;
            if (!m.IsEnabled && m.Icon != null)
            {
                (m.Icon as Image).Opacity = 0.3;
            }
            menu.Items.Add(m);

            // Need to have a seperate menu for each item, even if it is empty.
            // If ContextMenu is null, the parent's contextmenu will be used (WTF).
            // Item[0] is the menu title, Item[1] a Seperator, both always created.
            ContextMenu = menu;
            if (menu.Items.Count <= defaultMenuCount)
            {
                ContextMenu.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void AttribDelete_Click(object sender, RoutedEventArgs e)
        {
            KmlAttrib attrib = ((sender as MenuItem).DataContext as KmlAttrib);
            if (DlgConfirmation.Show("Do your really want to delete this attribute?\n" + attrib, "DELETE attribue", Icons.Delete))
            {
                attrib.Delete();
                // View will be refreshed in parent's ChildrenChanged event
            }
        }

    }
}
