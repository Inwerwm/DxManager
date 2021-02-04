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
    }
}
