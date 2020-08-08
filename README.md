# Altseed2.PublishTest



```sh
$ dotnet tool restore
$ dotnet fake build -t publish
$ dotnet fake build -t getsize
```

| Project | Runtime | Size |
| ---- | ---- | ---- |
| PublishSingleTrimmedNugetLocal | win-x64 | 33796004 |
| PublishSingleTrimmedConditionalGitHub | win-x64 | 34122015 |
| PublishSingleTrimmedNugetLocal | osx-x64 | 42023203 |
| PublishSingleTrimmedConditionalGitHub | osx-x64 | 42339998 |
| PublishSingleTrimmedNugetLocal | linux-x64 | 47862422 |
| PublishSingleTrimmedConditionalGitHub | linux-x64 | 48179233 |
| PublishSingleTrimmed | win-x64 | 52899368 |
| PublishSingleTrimmedExcludeNuget | win-x64 | 52900500 |
| PublishSingleTrimmedNugetLocal | win-x64 | 52962574 |
| PublishSingleTrimmed | osx-x64 | 57881122 |
| PublishSingleTrimmedExcludeNuget | osx-x64 | 57881726 |
| PublishSingleTrimmedNugetLocal | osx-x64 | 57953560 |
| PublishSingleTrimmed | linux-x64 | 62369527 |
| PublishSingleTrimmedExcludeNuget | linux-x64 | 62370131 |
| PublishSingleTrimmedNugetLocal | linux-x64 | 62441953 |
| PublishDefault | win-x64 | 94195053 |
| PublishDefault | osx-x64 | 98871553 |
| PublishDefault | linux-x64 | 103266354 |


## Dependencies
Altseed2-csharp
https://github.com/altseed/Altseed2-csharp/tree/566d0c12d0d89a1e74e0f9021ab83f52693a669b

(nuget: 0.0.4-alpha)
