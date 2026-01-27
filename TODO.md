# Project Name - TODO

A list of planned features, improvements, and tasks for this project.

> **CPX (Complexity Points)** - 1 to 5 scale:
> - **1** - Single file control/component
> - **2** - Single file control/component with single function change dependencies
> - **3** - Multi-file control/component or single file with multiple dependencies, no architecture changes
> - **4** - Multi-file control/component with multiple dependencies and significant logic, possible minor architecture changes
> - **5** - Large feature spanning multiple components and subsystems, major architecture changes

> Instructions for the TODO list:
- Move all completed items into separate Completed section
- Consolidate all completed TODO items by combining similar ones and shortening the descriptions where possible

> How TODO file should be iterated:
- First handle the Uncategorized section, if any similar issues already are on the TODO list, increase their priority instead of adding duplicates (categorize all at once)
- When Uncategorized section is empty, start by fixing Active Bugs (take one at a time)
- After Active Bugs, handle the rest of the TODO file by priority and complexity (High priority takes precedance, then CPX points) (take one at a time).

---

## Features

### High Priority

*No high priority items*

### Medium Priority

*No medium priority items*

### Lower Priority

*No lower priority items*

---

## Improvements

### Medium Priority

*No medium priority items*

---

## Documentation **LOW PRIORITY**

- [ ] API reference documentation
- [ ] Getting started guide
- [ ] Architecture overview

---

## Code Cleanup & Technical Debt

### Medium Priority

- [ ] **CPX 4** - Update C# bindings to latest Nuklear2 version

### Code Refactoring

*No refactoring items*

---

## Known Issues / Bugs

### Active Bugs

- Build fails to compile

### Uncategorized (Analyze and create TODO entries in above appropriate sections with priority. Do not fix or implement them just yet. Assign complexity points where applicable. Do not delete this section when you are done, just empty it)

- Example_Raylib project needs to be added to NuklearDotNetDotnet.sln (use Visual Studio UI)


---

## Notes

- Try to edit files and use tools WITHOUT POWERSHELL where possible, shell scripts get stuck and then manually terminate
- C# projects should target .NET 9
- Nuklear2 is the latest version of the original native GUI library Nuklear
- Keep small project summary as a separate section here (Summary) and up to date to keep track

---

## Completed

### Features

*No completed features yet*

### Improvements

- [x] **CPX 3** - Created Raylib backend using raylib_cs NuGet package (v6.1.1), removed legacy bundled bindings

### Fixed Bugs

- [x] **CPX 2** - NU1201: Updated target framework to net9.0 in Example_SFML, Example_MonoGame, Example_WindowsForms
- [x] **CPX 3** - NU1105: Fixed incorrect project reference paths (ExampleShared.csproj → ExampleSharedDotnet.csproj, NuklearDotNet.csproj → NuklearDotNetDotnet.csproj)
