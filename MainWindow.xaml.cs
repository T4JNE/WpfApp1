using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;

namespace WpfApp1
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string CurrentFileDir = null;
        private bool TextUnsaved = false;
        private bool FirstStartup = true;
        public MainWindow()
        {
            InitializeComponent();
            bool TextUnsaved = false;
            ChangeTitle(TextUnsaved);
            FirstStartup = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (UnsavedWarning())
            {
                e.Cancel = true;
            }
        }

        private bool UnsavedWarning()
        {
            if (TextUnsaved)
            {
                if (MessageBox.Show("Are you sure? Unsaved data will be lost!", "Warning",MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return true;
                }
            }
            return false;
        }

        private void ChangeTitle(bool isUnsaved)
        {
            if (CurrentFileDir != null)
            {
                this.Title = CurrentFileDir + " - Notepadzik";
            }
            else
            {
                this.Title = "No title - Notepadzik";
            }

            if (isUnsaved)
            {
                this.Title += " !UNSAVED!";
            }
        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (FirstStartup)
                return;


            TextUnsaved = true;
            ChangeTitle(TextUnsaved);
        }

        /*#########################################################
         * FILE BUTTONS BELOW
         *#########################################################*/

        private void New_Click(object sender, EventArgs e)
        {
            if (UnsavedWarning())
                return;

            CurrentFileDir = null;
            TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
            range.Text = "";
            TextUnsaved = false;
            ChangeTitle(TextUnsaved);
        }

        private void Open_Click(object sender, EventArgs e)
        {
            if (UnsavedWarning())
                return;

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Open";
            openFile.InitialDirectory = CurrentFileDir;
            openFile.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFile.Multiselect = false;
            if (openFile.ShowDialog() == true)
            {
                CurrentFileDir = openFile.FileName;
                TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                range.Text = File.ReadAllText(openFile.FileName);
                TextUnsaved = false;
                ChangeTitle(TextUnsaved);
            }
        }

        private void SaveAsWindow()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Save as";
            saveFile.InitialDirectory = CurrentFileDir;
            saveFile.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFile.ShowDialog() == true)
            {
                CurrentFileDir = saveFile.FileName;
                TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                File.WriteAllText(CurrentFileDir, range.Text);
            }
        }

        private void Save_as_Click(object sender, EventArgs e)
        {
            SaveAsWindow();
            TextUnsaved = false;
            ChangeTitle(TextUnsaved);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (CurrentFileDir != null)
            {
                TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                File.WriteAllText(CurrentFileDir, range.Text);
            }
            else
            {
                SaveAsWindow();
            }

            TextUnsaved = false;
            ChangeTitle(TextUnsaved);
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        /*#########################################################
         * EDIT BUTTONS BELOW
         *#########################################################*/

        private void Copy_Click(object sender, EventArgs e)
        {
            txtBox.Copy();
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            txtBox.Paste();
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            txtBox.Cut();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            txtBox.Selection.Text = "";
        }

        /*#########################################################
         * FORMAT BUTTONS BELOW
         *#########################################################*/
        private void Font_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FontDialog fd = new System.Windows.Forms.FontDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtBox.FontFamily = new FontFamily(fd.Font.Name);
                txtBox.FontSize = fd.Font.Size * 96.0 / 72.0;
                txtBox.FontWeight = fd.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
                txtBox.FontStyle = fd.Font.Italic ? FontStyles.Italic : FontStyles.Normal;

                TextDecorationCollection tdc = new TextDecorationCollection();
                if (fd.Font.Underline) tdc.Add(TextDecorations.Underline);
                if (fd.Font.Strikeout) tdc.Add(TextDecorations.Strikethrough);
            }
        }
    }
}
