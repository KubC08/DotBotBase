# DotBot Base
DotBot Base is a Discord bot "base" or "framework" written in C# using a fully modular design with built-in APIs that allows you to simply create/add modules to the bot that you would like, set up the configuration and get it running.

> **NOTE**: A lot of the features and APIs are still being worked on and are subject to change at the moment.

## How to install
1. Download the ZIP file from the Releases (_First release still pending_)
2. Extract the ZIP file to any location you would like
3. Modify the settings.json file located in the config directory
4. Run Discord.App
> Please note that if the config directory does not exist, then you have to run Discord.App once so it generates.

## How to install module/library
> Please be careful of potentially malicious modules and libraries, make sure you trust the source you are downloading from.
1. Make sure you have the "modules" and "libraries" directory (if not check "How to install")
2. When downloading a module or library make sure that you get a .dll file. If you have a .zip or .rar file you might have to extract them
3. Put the .dll files into either the "modules" directory or "libraries" directory, depending on what you need to install
4. Run the bot

## Requirements
- .NET 8 or higher

## API
The API currently consists of the following modules:
- Module loading/unloading
- Logging
- Commands

With planned APIs:
- Database

If you have any further suggestions or questions in regards to the API, please feel free to put them in the Issues tab. The documentation is still being worked on.
> If you would like to see how certain things work you can look at the "DotBotBase.Tests" code, to see a basic module example.

## Creating a module
```c#
using DotBotBase.Core.Modular;

namespace DotBotBase.Test;
public class TestModule : BotModule
{
    public override string Name => "Test Module";
    public override string Version => "1.0.0";
    public override string Author => "KubC08";

	public override void Start() {}
	public override void Shutdown() {}
}
```

## Credits
- [Discord.NET](https://discordnet.dev/index.html)
