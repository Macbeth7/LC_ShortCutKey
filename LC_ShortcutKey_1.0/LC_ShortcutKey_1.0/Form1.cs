using LC_ShortcutKey_1._0.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LC_ShortcutKey_1._0
{
    public partial class Form1 : Form
    {
        private ShortcutKey shortcutKey;

        private bool dragging = false;
        private bool Is_shortcutKey_stop = false;

        public Form1()
        {
            InitializeComponent();            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            shortcutKey = new ShortcutKey();
            shortcutKey.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            shortcutKey.End();
        }

        private void Form1_MouseHover(object sender, EventArgs e)
        {
            if (Is_shortcutKey_stop)
            {
                BackgroundImage = Resources.Uibright;
            }
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            if (Is_shortcutKey_stop)
            {
                BackgroundImage = Resources.Ui;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dragging = true;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Is_shortcutKey_stop)
            {
                BackgroundImage = Resources.Uibright;
            }

            if (e.Button == MouseButtons.Right)
            {
                if (dragging)
                {
                    Point mousePosition = Cursor.Position;
                    this.Location = new Point(mousePosition.X - 36, mousePosition.Y -36);
                }
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Is_shortcutKey_stop)
                {
                    shortcutKey.Resume();
                    BackgroundImage = Resources.Uibright;
                }
                else
                {
                    shortcutKey.Stop();
                    BackgroundImage = Resources.Ui;
                }

                Is_shortcutKey_stop = !Is_shortcutKey_stop;
            }
        }
    }
}
