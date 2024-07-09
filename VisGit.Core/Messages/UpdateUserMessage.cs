using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace VisGitCore.Messages
{
    internal class UpdateUserMessage : ValueChangedMessage<string>
    {

        public Exception Exception { get; set; }
        public object AssociatedObject { get; set; }
        public UpdateUserMessage(string value) : base(value)
        {
        }
    }
}