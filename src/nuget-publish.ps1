cls

cd $PSScriptRoot

$id = ((Get-Item -Path ".\..").Name)
& MSBuild.exe /t:clean
& MSBuild.exe /t:restore
& MSBuild.exe /t:pack /p:Configuration=Release

$package_file = @(Get-ChildItem "$id\bin\Release\*.nupkg" -Exclude "*.symbols.*" | Sort-Object -Property CreationTime -Descending)[0]
$package_file.Name

& nuget.exe push $package_file.FullName -source nuget.org

$package_file | Remove-Item