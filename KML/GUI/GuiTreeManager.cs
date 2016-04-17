﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace KML
{
    /// <summary>
    /// The GuiTreeManager combines a TreeView with a Listview.
    /// Into the TreeView all nodes from a KSP persistence file can be loaded.
    /// The attributes of the selected node are displayed in the details ListView.
    /// </summary>
    class GuiTreeManager : IGuiManager
    {
        private static string _oldSearchText = "";
        private static List<KmlItem> _oldSearchList = new List<KmlItem>();

        private GuiTabsManager Master { get; set; }
        private List<KmlItem> KmlRoots { get; set; }
        private TreeView Tree { get; set; }
        private ListView TreeDetails { get; set; }

        /// <summary>
        /// Creates a GuiTreeManager to link and manage the given TreeView and ListView.
        /// </summary>
        /// <param name="master">The master GuiTabsManager</param>
        /// <param name="tree">The TreeView to manage</param>
        /// <param name="treeDetails">The ListView to manage</param>
        public GuiTreeManager(GuiTabsManager master, TreeView tree, ListView treeDetails)
        {
            Master = master;
            KmlRoots = new List<KmlItem>();
            Tree = tree;
            TreeDetails = treeDetails;

            Tree.SelectedItemChanged += Tree_SelectedItemChanged;
        }

        /// <summary>
        /// Retrieve a flat list of alle items found by recursive walk through whole tree,
        /// where item is of type T (or derivative).
        /// </summary>
        /// <typeparam name="T">They type derived from KmlItem to search for</typeparam>
        /// <returns>A List of alle items with type T</returns>
        public List<T> GetFlatList<T>() where T : KmlItem
        {
            return GetFlatList<T>(null);
        }

        private List<T> GetFlatList<T>(KmlNode parent) where T : KmlItem
        {
            List<KmlItem> source;
            if (parent == null)
            {
                source = KmlRoots;
            }
            else
            {
                source = parent.AllItems;
            }
            List<T> target = new List<T>();
            foreach (KmlItem item in source)
            {
                if (item is T)
                {
                    target.Add((T)item);
                }
                if (item is KmlNode)
                {
                    target.AddRange(GetFlatList<T>((KmlNode)item));
                }
            }
            return target;
        }

        /// <summary>
        /// Load a KML structure from data file into tree.
        /// </summary>
        /// <param name="filename">The full path and filename of the data file to read</param>
        public void Load(string filename)
        {
            Tree.Items.Clear();
            TreeDetails.Items.Clear();

            KmlRoots = KmlItem.ParseFile(filename);

            // Check if any of these roots is not a node
            // If so, pack all roots into a new ghost root
            if (KmlRoots.Any(x => !(x is KmlNode)))
            {
                KmlGhostNode root = new KmlGhostNode(System.IO.Path.GetFileName(filename));
                root.AddRange(KmlRoots);
                KmlRoots.Clear();
                KmlRoots.Add(root);
            }

            foreach (KmlItem item in KmlRoots)
            {
                if (item is KmlNode)
                {
                    item.CanBeDeleted = false;
                    Tree.Items.Add(new GuiTreeNode((KmlNode)item));
                }
            }

            if (Tree.Items.Count >= 1)
            {
                TreeViewItem item = (TreeViewItem)Tree.Items[0];
                item.IsSelected = true;
                if (Tree.Items.Count == 1)
                {
                    item.IsExpanded = true;
                }
            }
        }

        /// <summary>
        /// Save the KML structure from tree into data file.
        /// </summary>
        /// <param name="filename">The full path and filename of the data file to write</param>
        public void Save(string filename)
        {
            KmlItem.WriteFile(filename, KmlRoots);
        }

        /// <summary>
        /// Seach for text in the KML structure.
        /// </summary>
        /// <param name="text">The text to search for</param>
        /// <param name="checkNodeTag">If node tags are searched in</param>
        /// <param name="checkNodeText">If node display texts are searched in</param>
        /// <param name="checkAttribName">If attribute names are searched in</param>
        /// <param name="checkAttribValue">If attribute values are searched in</param>
        /// <returns>A list of found KmlItems</returns>
        public List<KmlItem> Search(string text, bool checkNodeTag = true, bool checkNodeText = true, bool checkAttribName = true, bool checkAttribValue = true)
        {
            List<KmlItem> result = new List<KmlItem>();
            if (_oldSearchText.Length > 0 && text.ToLower().Contains(_oldSearchText.ToLower()))
            {
                List<KmlItem> seachList = _oldSearchList;
                _oldSearchList = new List<KmlItem>();
                foreach (KmlItem item in seachList)
                {
                    SearchAddItem(result, item, text, checkNodeTag, checkNodeText, checkAttribName, checkAttribValue);
                }
            }
            else
            {
                foreach (KmlNode node in KmlRoots)
                {
                    SearchAddItem(result, node, text, checkNodeTag, checkNodeText, checkAttribName, checkAttribValue);
                    SearchRecursive(result, node, text, checkNodeTag, checkNodeText, checkAttribName, checkAttribValue);
                }
                _oldSearchList = result;
            }
            _oldSearchText = text;
            return result;
        }

        private void SearchRecursive(List<KmlItem> result, KmlNode node, string text, bool checkNodeTag, bool checkNodeText, bool checkAttribName, bool checkAttribValue)
        {
            if (checkAttribName || checkAttribValue)
            {
                foreach (KmlAttrib attrib in node.Attribs)
                {
                    SearchAddItem(result, attrib, text, checkNodeTag, checkNodeText, checkAttribName, checkAttribValue);
                }
            }
            foreach (KmlNode child in node.Children)
            {
                SearchAddItem(result, child, text, checkNodeTag, checkNodeText, checkAttribName, checkAttribValue);
                SearchRecursive(result, child, text, checkNodeTag, checkNodeText, checkAttribName, checkAttribValue);
            }
        }

        private bool SearchCheckItem(KmlItem item, string text, bool checkNodeTag, bool checkNodeText, bool checkAttribName, bool checkAttribValue)
        {
            if (item is KmlNode)
            {
                KmlNode node = (KmlNode)item;
                return (checkNodeTag && node.Tag.ToLower().Contains(text.ToLower())) ||
                    (checkNodeText && node.ToString().ToLower().Contains(text.ToLower()));
            }
            else if (item is KmlAttrib)
            {
                KmlAttrib attrib = (KmlAttrib)item;
                return (checkAttribName && attrib.Name.ToLower().Contains(text.ToLower())) ||
                    (checkAttribValue && attrib.Value.ToLower().Contains(text.ToLower()));
            }
            else
            {
                return false;
            }
        }

        private void SearchAddItem(List<KmlItem> result, KmlItem item, string text, bool checkNodeTag, bool checkNodeText, bool checkAttribName, bool checkAttribValue)
        {
            if (SearchCheckItem(item, text, checkNodeTag, checkNodeText, checkAttribName, checkAttribValue))
            {
                result.Add(item);
            }
            if (SearchCheckItem(item, text, true, true, true, true))
            {
                _oldSearchList.Add(item);
            }
        }

        /// <summary>
        /// Focus the standard control. Also select first item in the tree, 
        /// if there is one and none is selected.
        /// </summary>
        public void Focus()
        {
            if (Tree.SelectedItem == null)
            {
                if (Tree.Items.Count > 0)
                {
                    (Tree.Items[0] as TreeViewItem).IsSelected = true;
                }
            }
            if (Tree.SelectedItem != null)
            {
                Application.Current.MainWindow.UpdateLayout();
                (Tree.SelectedItem as TreeViewItem).Focus();
            }
        }

        /// <summary>
        /// Selects next sibling of currently selected GuiTreeNode.
        /// It's not leaving current level of hierarchy (yet).
        /// </summary>
        public void Next()
        {
            Focus();
            TreeViewItem selected = Tree.SelectedItem as TreeViewItem;
            if (selected != null)
            {
                TreeSelect(selected.Parent as TreeViewItem, GetTreeSelectedIndex() + 1);
                // TODO GuiTreeManager.Next(): Change hierarchy after last node.
                Focus();
            }
        }

        /// <summary>
        /// Selects previous sibling of currently selected GuiTreeNode.
        /// It's not leaving current level of hierarchy (yet).
        /// </summary>
        public void Previous()
        {
            Focus();
            TreeViewItem selected = Tree.SelectedItem as TreeViewItem;
            if (selected != null)
            {
                TreeSelect(selected.Parent as TreeViewItem, GetTreeSelectedIndex() - 1);
                // TODO GuiTreeManager.Previous(): Change hierarchy before first node.
                Focus();
            }
        }

        /// <summary>
        /// Select the given item in the tree view.
        /// </summary>
        /// <param name="item">The KmlItem to select</param>
        public void Select(KmlItem item)
        {
            // Stack will contain items parent nodes
            Stack<KmlNode> stack = new Stack<KmlNode>();

            // Search in KML, root node goes first onto the stack
            foreach (KmlNode node in KmlRoots)
            {
                stack.Push(node);
                if (SelectSearch(stack, item))
                {
                    break;
                }
                else
                {
                    stack.Pop();
                }
            }

            // This reverses the stack, so root node comes out first
            stack = new Stack<KmlNode>(stack);

            // Master node will be selected later, 
            // if item is KmlNode, master = GuiTreeNode for item
            // otherwise, master = GuiTreeNode for item.Parent
            GuiTreeNode masterNode = null;

            // Expand recursively all parent nodes from stack, root first
            // Seach root in the Tree.Items
            foreach (GuiTreeNode treeNode in Tree.Items)
            {
                if (stack.Count > 0)
                {
                    KmlNode peek = stack.Peek();
                    if (treeNode.DataNode == peek)
                    {
                        stack.Pop();
                        masterNode = treeNode;
                        treeNode.IsExpanded = true;
                        break;
                    }
                }
                else if (treeNode.DataNode == item)
                {
                    masterNode = treeNode;
                    break;
                }
            }

            if (masterNode == null)
            {
                // Item found nowhere
                return;
            }

            // Seach following nodes recursive in parent treeNode
            foreach (KmlNode node in stack)
            {
                foreach (GuiTreeNode treeNode in masterNode.Items)
                {
                    if (treeNode.DataNode == node)
                    {
                        // No need to pop, this is handled by foreach loop
                        masterNode = treeNode;
                        treeNode.IsExpanded = true;
                        break;
                    }
                }
            }

            // And now we can select and focus the item if it is a node, its parent otherwise
            if (item is KmlNode && masterNode.DataNode != item)
            {
                foreach (GuiTreeNode sub in masterNode.Items)
                {
                    if (sub.DataNode == item)
                    {
                        sub.BringIntoView();
                        // Force a refreh, by causing SelectionChanged to invoke
                        sub.IsSelected = false;
                        sub.IsSelected = true;
                        Focus();
                        break;
                    }
                }
            }
            else
            {
                masterNode.BringIntoView();
                // Force a refreh, by causing SelectionChanged to invoke
                masterNode.IsSelected = false;
                masterNode.IsSelected = true;
                Focus();
            }
        }

        /// <summary>
        /// Get the selected KmlItem. Will be needed to check if
        /// views have to be refreshed.
        /// </summary>
        /// <returns>The currently selected KmlItem</returns>
        public KmlItem GetSelectedItem()
        {
            if (Tree.SelectedItem is GuiTreeNode)
            {
                return (Tree.SelectedItem as GuiTreeNode).DataNode;
            }
            return null;
        }

        private bool SelectSearch(Stack<KmlNode> stack, KmlItem item)
        {
            KmlNode node = stack.Peek();
            foreach (KmlItem sub in node.AllItems)
            {
                if (sub == item)
                {
                    return true;
                }
                else if (sub is KmlNode)
                {
                    stack.Push(sub as KmlNode);
                    if (SelectSearch(stack, item))
                    {
                        return true;
                    }
                    else
                    {
                        stack.Pop();
                    }
                }
            }
            return false;
        }

        private ItemCollection GetTreeItemCollection(TreeViewItem item)
        {
            if (item != null)
            {
                return item.Items;
            }
            else
            {
                return Tree.Items;
            }
        }

        private int GetTreeSelectedIndex()
        {
            TreeViewItem selected = Tree.SelectedItem as TreeViewItem;
            ItemCollection list = GetTreeItemCollection(selected.Parent as TreeViewItem);
            return list.IndexOf(Tree.SelectedItem);
        }

        private bool TreeSelect(TreeViewItem parent, int selectIndex)
        {
            ItemCollection list = GetTreeItemCollection(parent);
            if (selectIndex >= 0 && selectIndex < list.Count)
            {
                (list[selectIndex] as TreeViewItem).IsSelected = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeDetails.Items.Clear();
            if (Tree.SelectedItem != null)
            {
                GuiTreeNode Node = (GuiTreeNode)Tree.SelectedItem;
                TreeDetails.ContextMenu = Node.ContextMenu;
                foreach (KmlAttrib attrib in Node.DataNode.Attribs)
                {
                    TreeDetails.Items.Add(new GuiTreeAttrib(attrib));
                }
            }
            else
            {
                TreeDetails.ContextMenu = null;
            }
        }

    }
}
