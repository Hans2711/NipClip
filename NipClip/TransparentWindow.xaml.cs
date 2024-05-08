using NipClip.Classes.Keyboard;
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
using System.Windows.Shapes;

namespace NipClip
{
    /// <summary>
    /// Interaction logic for TransparentWindow.xaml
    /// </summary>
    public partial class TransparentWindow : Window
    {
        public bool active = false;

        public string activeAction = string.Empty;

        public TransparentWindow()
        {
            this.ShowActivated = false;
            SizeChanged += (o, e) =>
            {
                var r = SystemParameters.WorkArea;
                Left = r.Right - ActualWidth;
                Top = r.Bottom - ActualHeight;
            };
            InitializeComponent();
        }

        public void Clean()
        {
            this.activeAction = string.Empty;
            this.active = false;
            this.label.Content = "[]";
        }

        public bool keyboardHook_KeyDown(KeyboardHook.VKeys key)
        {

            return false;
        }


        public void StartCopyAction()
        {
            this.activeAction = "copy";
            this.label.Content = "copy";
        }

        public void StartPasteAction()
        {
            this.activeAction = "paste";
            this.label.Content = "paste";
        }
    }
}
