# TestFiles
These are test files used as input, expected content etc...

They are `Embedded Resources` that are retrievable by `Assembly.GetManafiestResourceStream()`.
To simplify things the `TestHelpers.GetTestResourceStream()` and `TestHelpers.GetTestText()`
class provides overloads that follow the patterns described here. Additionally, each test
class has a static method `GetSourceText(params string[] nameParts)` that automatically
provides the `<Test Class Name>` part of the pattern. This makes it very simple to load a
resource stream and get the contents as a `SourceText` for use in testing.

## Naming Patterns

```
📁 TestFiles
├─📁 <Test Class Name>
│  ├─📁 <Test Method Name>
│  │ ├─📄 <file1.ext>
│  │ └─📄 <filen.ext>
│  ├─📁 <Test Method Name 2>
│  │ ├─📄 <file1.ext>
│  │ └─📄 <filen.ext>
├─📁 <Test Class Name>
   └─ ...
```

>[!IMPORTANT]
> It is important to note that .NET considers resource names as case sensitive. While
> Microsoft Windows normally uses a case preserving but insensitive match for file names the
> resource names are captured from the underlying FS and are case preserving. Thus, the case
> of file names ***MUST*** match the name used to retrieve it as a resource.

### Final Resource Name
The full resource name pattern is:  
`<Assembly Default Namespace>.<TestFiles Folder Name>.<Test Class Name>[.<Test method name>].<FileName>`

where:

| Name | Description |
|------|-------------|
| `<Assembly Default Namespace>` | Default namespace for the assembly (Technically the namespace that contains `TestHelpers`.|
| `<TestFiles Folder Name>` | The name of the folder containing this file. |
| `<Test Class Name>` | The name of the test class (as a sub-folder of the one containing this file) |
| `<Test Method Name>` | The name of the test method  [Optional] (As a sub folder of the test class name)|
| `<FileName>` | The name of the file used by the tests. |

This ensures the contents are easily retrieved using a defined pattern and (`nameof(T)`)
along with the name of the file of interest. Everything else is formed from the pattern and
doesn't change for each file/test.

