using System.ComponentModel;
using System.Runtime.InteropServices;
using Community.VisualStudio.Toolkit;

namespace VisGitCore.Services
{
    public partial class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.UserSettingsOptions), "VisGit.Services", "UserSettings", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class UserSettingsOptions : BaseOptionPage<UserSettings>
        { }
    }

    public class UserSettings : BaseOptionModel<UserSettings>
    {
        [Category("My category")]
        [DisplayName("My Option")]
        [Description("An informative description.")]
        [DefaultValue(true)]
        public bool MyOption { get; set; } = true;
    }
}