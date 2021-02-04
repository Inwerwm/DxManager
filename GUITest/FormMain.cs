using DxManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUITest
{
    public partial class FormMain : Form
    {
        public DxContext DxContext { get; }

        public FormMain()
        {
            InitializeComponent();

            DxContext = DxContext.GetInstance(splitContainer1.Panel2);
        }

        private void splitContainer1_Panel2_ClientSizeChanged(object sender, EventArgs e)
        {
            DxContext?.ChangeResolution();
        }

        private void buttonFps10_Click(object sender, EventArgs e)
        {
            DxContext.RefreshRate = 10;
        }

        private void buttonFps60_Click(object sender, EventArgs e)
        {
            DxContext.RefreshRate = 60;
        }

        private void buttonFps144_Click(object sender, EventArgs e)
        {
            DxContext.RefreshRate = 144;
        }

    }
}
