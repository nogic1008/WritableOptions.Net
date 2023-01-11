# ConsoleAppExample

Example of use `WritableOptions` with [ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework).

As the sample does, it is useful for recording values like the latest launch datetime.

## Usage

Need [appsettings.json](./appsettings.json) in current folder.

```console
> dotnet run
currentOption: { LastChanged: 2021/01/21 0:00:00, ApiKey : 00000000-0000-0000-0000-000000000000 }
newOption: { LastChanged: 2021/02/23 7:30:00, ApiKey : 4b45b238-ff1c-4faf-93e9-4015f4dd3f48 }
Try to write new settings.
Success! Check your appsettings.json.
writableOptions.Value: { LastChanged: 2021/02/23 7:30:00, ApiKey : 4b45b238-ff1c-4faf-93e9-4015f4dd3f48 }
```

## See also

- [.NET Generic Host in ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/host/generic-host)
