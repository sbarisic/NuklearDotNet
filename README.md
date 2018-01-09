# NuklearDotNet
LICENSE: Dual-licensed under MIT and The Unlicense. Your choice.

.NET binding for the Nuklear immediate mode GUI

NuklearSharp ( https://github.com/leafi/NuklearSharp ) was my original inspiration for this, i am not a fan
of the original way the bindings were loaded. It was too much code duplication and i don't really understand
why P/Invoke wasn't used.

No original nuklear source files were modified. A project was created with some support code 
to build Nuklear.dll (x64) with all API functions exported and ready to be used from .NET

Updating should be as easy as updating the submodule and rebuilding the project. 