using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPReceiver
{
    public interface IFinger : IDisposable
    {
        void Load();
    }
}
