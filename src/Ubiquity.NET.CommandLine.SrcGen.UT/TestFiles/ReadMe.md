# TestFiles
These are test files used as input, expected content etc...

They are `Embedded Resources` that are retrievable by `Assembly.GetManafiestResourceStream()`.
To simplify things the `TestHelpers` class provides overloads that follow the patterns
described here.

## Naming Patterns

```
📁 TestFiles
└─📁 <TestName>
   ├─📄 <file1.ext>
   └─📄 <filen.ext>
```
Where `<TestName>` is the name of the test (in code that is usually `nameof(T)`).

>[!IMPORTANT]
> It is important to note that .NET considers resource names as case sensitive. While
> Microsoft Windows normally uses a case preserving but insensitive match for file names the
> resource names are captured from the underlying FS and are case preserving. Thus, the case
> of file names ***MUST*** match the name used to retrieve it as a resource.

### Final Resource Name
The full resource name pattern is:  
`<Assembly Default Namespace>.<TestFiles Folder Name>.<TestName>.<FileName>`  

where:

| Name | Description |
|------|-------------|
| `<Assembly Default Namespace>` | Default namespace for the assembly (Technically the namespace that contains `TestHelpers`.|
| `<TestFiles Folder Name>` | The name of the folder containing this file. |
| `<TestName>` | The name of the tests (as a sub-folder of the one containing this file) |
| `<FileName>` | The name of the file used by the tests. |

This ensures the contents are easily retrieved using just a test name (via `nameof(T)`) and
the name of the file of interest. Everything else is formed from the pattern and doesn't
change for each file/test.
