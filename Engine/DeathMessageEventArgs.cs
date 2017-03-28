using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class DeathMessageEventArgs : EventArgs
    {
        public string Message { get; set; }

        public DeathMessageEventArgs(string Message)
        {
            this.Message = Message;
        }
    }
}
