using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class MessageEventArgs : EventArgs
    {
        public string Message { get; set; }
        public bool AddNewLine { get; set; }

        public MessageEventArgs(string Message,bool AddNewLine)
        {
            this.Message = Message;
            this.AddNewLine = AddNewLine;
        }
    }
}
