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
            set => personalAccessToken = Encryption.DpapiEncryptString(Encryption.DpapiToSecureString(value));
        }
    }
}