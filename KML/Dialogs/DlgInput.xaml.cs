﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KML
{
    /// <summary>
    /// A DlgInput is used to display a message and let the user input one line of text.
    /// </summary>
    public partial class DlgInput : Window
    {
        private DlgInput(string message, string title, Image image, string presetText)
        {
            InitializeComponent();

            if (title != null)
            {
                Title = title;
            }
            if (image != null)
            {
                Icon = image.Source;
            }
            if (presetText != null)
            {
                TextBoxInput.Text = presetText;
            }
            else
            {
                TextBoxInput.Text = "";
            }
            TextMessage.Text = message;
            TextBoxInput.SelectionStart = TextBoxInput.Text.Length;
            TextBoxInput.Focus();

            DlgHelper.Initialize(this);
            DlgHelper.CalcNeededSize(this, TextMessage, ButtonOk.Height + TextBoxInput.Height);
        }

        private DlgInput(string message, string title, Image image)
            : this(message, title, image, null)
        {
        }

        private DlgInput(string message, string title)
            : this (message, title, null, null)
        {
        }

        private DlgInput(string message)
            : this (message, null, null, null)
        {
        }

        /// <summary>
        /// Show a dialog window with given title and message.
        /// </summary>
        /// <param name="message">The message to show</param>
        /// <param name="title">The window title</param>
        /// <param name="presetText">The preset text for the input</param>
        /// <param name="output">Out: Returns the input text if "Ok" was clicked, the preset text otherwise</param>
        /// <returns>True if "Ok" was clicked, false otherwise</returns>
        public static bool Show(string message, string title, string presetText, out string output)
        {
            DlgInput dlg = new DlgInput(message, title, null, presetText);
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                output = dlg.TextBoxInput.Text;
            }
            else
            {
                output = presetText;
            }
            return result == true;
        }

        /// <summary>
        /// Show a dialog window with given title and message.
        /// </summary>
        /// <param name="message">The message to show</param>
        /// <param name="presetText">The preset text for the input</param>
        /// <param name="output">Out: Returns the input text if "Ok" was clicked, the preset text otherwise</param>
        /// <returns>True if "Ok" was clicked, false otherwise</returns>
        public static bool Show(string message, string presetText, out string output)
        {
            return Show(message, null, presetText, out output);
        }

        /// <summary>
        /// Show a dialog window with given message.
        /// </summary>
        /// <param name="message">The message to show</param>
        /// <param name="output">Out: Returns the input text if "Ok" was clicked, null otherwise</param>
        /// <returns>True if "Ok" was clicked, false otherwise</returns>
        public static bool Show(string message, out string output)
        {
            return Show(message, null, null, out output);
        }

        /// <summary>
        /// Show a dialog window with input only.
        /// </summary>
        /// <param name="output">Out: Returns the input text if "Ok" was clicked, null otherwise</param>
        /// <returns>True if "Ok" was clicked, false otherwise</returns>
        public static bool Show(out string output)
        {
            return Show(null, null, null, out output);
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ButtonCancel.Width = ButtonOk.Width = (Content as Grid).ActualWidth / 2.0;
        }
    }
}
