function BuildWithStrongName
{
    # Backup files that will be changed
    Copy-Item "./Penneo.csproj" -Destination "./Penneo.csproj.backup";

    # Read project XML
    [xml]$csproj = Get-Content -Path ".\Penneo.csproj"

    # Create SignAssembly node
    $signAssembly = $csproj.CreateElement("SignAssembly");
    $signAssembly.InnerText = "true";
    $csproj.Project.PropertyGroup.AppendChild($signAssembly);

    # Create KeyFile node
    $AssemblyOriginatorKeyFile = $csproj.CreateElement("AssemblyOriginatorKeyFile");
    $AssemblyOriginatorKeyFile.InnerText = "keypair.snk";
    $csproj.Project.PropertyGroup.AppendChild($AssemblyOriginatorKeyFile);

    # Save project XML
    $csproj.Save("Penneo.csproj");

    # Build project and nuget package
    dotnet build -c Release Penneo.csproj

    # Restore backup
    Remove-Item ".\Penneo.csproj";
    Copy-Item "./Penneo.csproj.backup" -Destination "./Penneo.csproj";
    Remove-Item ".\Penneo.csproj.backup";
}

function Sign($assembly)
{
    &"C:\Program Files (x86)\Windows Kits\10\bin\x64\signtool.exe" sign /a /tr http://timestamp.comodoca.com/rfc3161 /td sha256 /fd sha256 /v /f "Cert.pfx" /p xyz /v $assembly
}

function Pack
{
    dotnet pack -c Release --no-build
}

# Build the project
BuildWithStrongName

# Sign the dlls
Sign("C:\temp\Penneo\bin\Release\net461\Penneo.dll")
Sign("C:\temp\Penneo\bin\Release\\netcoreapp2.0\Penneo.dll")

# Build the Nuget package
Pack
