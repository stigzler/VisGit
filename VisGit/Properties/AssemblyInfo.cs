using System.Reflection;
using System.Runtime.InteropServices;
using VisGit;

[assembly: AssemblyTitle(Vsix.Name)]
[assembly: AssemblyDescription(Vsix.Description)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(Vsix.Author)]
[assembly: AssemblyProduct(Vsix.Name)]
[assembly: AssemblyCopyright(Vsix.Author)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion(Vsix.Version)]
[assembly: AssemblyFileVersion(Vsix.Version)]

// My additions
[assembly: ProvideCodeBase(AssemblyName = "Microsoft.Xaml.Behaviors")]
[assembly: ProvideCodeBase(AssemblyName = "CommunityToolkit.Mvvm")]
[assembly: ProvideCodeBase(AssemblyName = "VisGit.Core")]
[assembly: ProvideCodeBase(AssemblyName = "Octokit")]
[assembly: ProvideCodeBase(AssemblyName = "Dsafa.WpfColorPicker")]
[assembly: ProvideCodeBase(AssemblyName = "WpfScreenHelper")]

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit
    { }
}