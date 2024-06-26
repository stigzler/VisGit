# Hacks/Workarounds

## Integration of MVVM Toolkit and VSIX

The two don't play nicely together, thus had to separate out VMs etc into a separate class. The `[ObservableProperty]` auto-generated Public properties whilst available through code do not compile. Did contact the author, but his solution was to separate out the concerns. Links:

[Github](https://github.com/CommunityToolkit/dotnet/issues/889) | 
[StackOverflow](https://stackoverflow.com/questions/78619310/build-errors-when-using-communitytoolkit-mvvm-with-visual-studio-extensions?noredirect=1#comment138656325_78619310)

## VisGit.OptionsPage

Due to above, tried to put the OptionsPage in VisGit.Core. However, no end of compile errors - likely to do with VSIX. Tried to access this in VisGit via interface, but again, got strange errors at compile time. Thus, had to leverage the Settings system. 

[StackOverflow](https://stackoverflow.com/questions/78655570/creating-an-optionsprovider-in-a-separate-project-library-to-the-vsix-project-in)

