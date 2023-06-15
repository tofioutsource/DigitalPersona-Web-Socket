using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace DPReceiver
{
    [RunInstaller(true)]
    public partial class FingerInstaller : System.Configuration.Install.Installer
    {
        public FingerInstaller()
        {
            InitializeComponent();
        }
    }
}
