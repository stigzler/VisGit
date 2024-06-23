using System.ComponentModel;
using System.Runtime.InteropServices;
using VisGit.Core.Services;

namespace VisGit
{
    internal partial class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.OptionsPageOptions), "VisGit", "OptionsPage", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class OptionsPageOptions : BaseOptionPage<OptionsPage>
        { }
    }

    /// <summary>
    /// HACK: Having to leverage Properties.Settings in VisGit.Core due to no end of issues
    /// splitting VMs into separate lib (due to mvvm toolkit peculiarities) and also VSIX environ
    /// making leveraging Interfaces weird.
    /// </summary>
    public class OptionsPage : BaseOptionModel<OptionsPage>
    {
        private string personalAccessToken = "{UNSET}";

        [Category("Github Settings")]
        [DisplayName("Personal Access Token")]
        [Description("Your GitHub Personal Access Token. Get one via Github > Settings > Developer Settings > Personal Access Tokens > Fine-grained tokens")]
        [DefaultValue("{UNSET}")]
        [PasswordPropertyText(true)]
        public string PersonalAccessToken
        {
            get => Encryption.DpapiToInsecureString(Encryption.DpapiDecryptString(personalAccessToken));
            set
            {
                personalAccessToken = Encryption.DpapiEncryptString(Encryption.DpapiToSecureString(value));
                Core.Properties.Settings.Default.PersonalAccessTokenEncrypted = personalAccessToken;
                Core.Properties.Settings.Default.Save();
            }
        }

        private string _testSetting;

        [Category("Github Settings")]
        [DisplayName("Test")]
        public string TestSetting
        {
            get { return _testSetting; }
            set
            {
                _testSetting = value;
                Core.Properties.Settings.Default.TestSetting = _testSetting;
                Core.Properties.Settings.Default.Save();
            }
        }
    }
}