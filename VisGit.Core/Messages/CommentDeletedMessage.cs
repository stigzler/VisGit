using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGitCore.ViewModels;

namespace VisGitCore.Messages
{
    internal class CommentDeletedMessage : ValueChangedMessage<IssueCommentViewModel>
    {
        public CommentDeletedMessage(IssueCommentViewModel value) : base(value)
        {
        }
    }
}