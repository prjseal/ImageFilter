# ImageFilter Content App for Umbraco

![Nuget](https://img.shields.io/nuget/v/ImageFilter)
[![Nuget Downloads](https://img.shields.io/nuget/dt/ImageFilter.svg)](https://www.nuget.org/packages/ImageFilter)

## Watch the video
[![Watch the video on YouTube](/images/image-filter-screenshot.png)](https://www.youtube.com/watch?v=LXtYAhZvUXk)


## Login details

<strong>Username:</strong> admin@admin.com
<strong>Password:</strong> 1234567890

## Dev instructions

Edit the files in project ImageFilter <strong>not</strong> the ImageFilter.Web project.
The files will copy over after you build the ImageFilter.Web project.

## Issues with Rosyln Compiler ##

When running the project for the first time you may get a runtime error stating that csc.exe could not be found. To resolve this a nuget package needs to be reinstalled. Use the following command to fix this issue:

`Update-package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -r`
