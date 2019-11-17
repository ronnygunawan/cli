### Installation
```
Install-Package RG.CLI
```

### Usage
```cs
ConsoleApp app = new ConsoleApp(exitCommand: "exit");
app["cls"] = args => Console.Clear();
app["echo {message}"] = args => Console.WriteLine(args[0]);
app["ls"] = args => FS.LS();
app["ls {pattern}"] = args => FS.LS(args[0]);
app["ls {pattern} {options}"] = args => FS.LS(args[0], args[1]);
app["cd {path}"] = args => FS.CD(args[0]);
app["pwd"] = args => FS.PWD();
app.Run();
```

### Tab to complete or list commands
`> ls` <kbd>Tab</kbd>
```
ls                       ls {pattern}
ls {pattern} {options}

> ls
```
