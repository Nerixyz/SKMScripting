# SKMScripting
Adds scripting support for SKMNet supported Lighting-Consoles by transtechnik/ETC

Run a script using `-s PATH_TO_SCRIPT.csx [-i IP_TO_SKM]`

An example can be found here: [GeneralExample.csx](https://github.com/Nerixyz/SKMScripting/blob/master/SKMScripting/Examples/GeneralExample.csx).

Currently, these are the global variables:
- LightingConsole (with Extensions) `console`
- Action<object, LogLevel> `Log`
- Action<object> `LogInfo`
- Action<object> `LogDebug`

# Common (Extension-) Methods

Most of the calls are async, so make sure to await them.

## Pal (I, F, C, B, BLK (scene), DYN, SKG)

### Select

Select[PAL](double number)

Select a scene:
`console.SelectBLK(double number);`
