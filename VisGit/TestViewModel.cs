using CommunityToolkit.Mvvm.ComponentModel;

namespace VisGit
{
    internal partial class TestViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _TestString;

        public TestViewModel()
        {
            TestString = "Test";
        }
    }
}