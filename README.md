# Live Process Inspector

***
Live Process Inspector builded around experimental [Microsoft.Diagnostics.Runtime] (https://github.com/Microsoft/dotnetsamples/tree/master/Microsoft.Diagnostics.Runtime/CLRMD) 
library to help inspecting a crash dump of a .NET programs. Also have experimental ability 
attach to live process.

## Features

- ability to open dump files from drive
- ability to open recently opened dump files
- ability to clear folder from all dump files
- ability to select live process just dragging target cursor over it
- ability to attach to live process (experimental)
- ability to create full dump using [procdump] (https://technet.microsoft.com/en-us/sysinternals/dd996900.aspx)
![Alt text](/LiveProcessInspector/LiveProcessInspector/screenshots/attachOrDebug.png)
- ability to inspect application compile architecture, pointer size, CLR versions, modules
![Alt text](/LiveProcessInspector/LiveProcessInspector/screenshots/dataTargetView.png)
- ability to open CLR location, module location
- ability to open inspect CLR runtime of application (heap count, GC type, pointer size, 
application domains, application threads)
- ability to select thread and see it main parameters (alive, STA, exception, aborted, background, finalized)
- ability to inspect application thread pool information
![Alt text](/LiveProcessInspector/LiveProcessInspector/screenshots/clrRuntime.png)
- ability to inspect application object heap (*experimental)
![Alt text](/LiveProcessInspector/LiveProcessInspector/screenshots/simpleObjectHeap.png)

## Typical usage
![demo] (LiveProcessInspector/LiveProcessInspector/screenshots/dumpAppScreenCast.gif)

### Requirenments
- Windows 7-8.1
- .NET Framework 4.6 (can be rebuilded to 4.0)

### Dependencies
- Caliburn.Micro
- [procdump.exe] (https://technet.microsoft.com/en-us/sysinternals/dd996900.aspx)
- Microsoft.Diagnostics.Runtime (* added as reference to local file, see discussion: https://github.com/Microsoft/dotnetsamples/issues/11)

### Author
Maksym Chernenko
[Email] (Maksym.Chernenko@gmail.com)
