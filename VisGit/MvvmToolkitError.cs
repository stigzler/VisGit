using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGit
{
    internal partial class MvvmToolkitError : ObservableObject
    {
        [ObservableProperty]
        private bool _works = false;

        public MvvmToolkitError()
        {
            Works = true;
        }
    }
}