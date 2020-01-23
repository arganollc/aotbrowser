<#
.SYNOPSIS
    Helps to mount GitHub repo to your machine
.DESCRIPTION
    It creates a symbolic link into Metadata folder of Dynamics 365 for Operations.
.COMPANY
    Arbela Technologies Corp.
#>


if (Test-Path -Path K:\AosService)
{
   $LocalDeploymentFolder = "K:\AosService"
}
elseif (Test-Path -Path C:\AosService)
{
   $LocalDeploymentFolder = "C:\AosService"
}
elseif (Test-Path -Path E:\AosService)
{
   $LocalDeploymentFolder = "E:\AosService"
}
else
{
  throw "Cannot find the AOSService folder in any known location"
}
Write-Host "using $LocalDeploymentFolder as the deployment folder"

$LocalMetadataFolder = Join-Path -Path $LocalDeploymentFolder -ChildPath "\PackagesLocalDirectory"

Write-Host "using $LocalMetadataFolder as the metadata folder"

# Get the list of models to junction
$ModelsToJunction = Get-ChildItem "..\Metadata"

Write-Host "Enabling editing of the following models:" $ModelsToJunction

foreach ($Model in $ModelsToJunction) 
{
    $LocalModelPath = Join-Path $LocalMetadataFolder $Model
    $RepoPath = Join-Path "..\Metadata" $Model
	
	if (!(Test-Path $LocalModelPath -PathType Container))
	{
		Write-Host "Creating model folder: " $LocalModelPath
		New-Item -ItemType Directory -Force -Path $LocalModelPath
	}

    $RepoSubfolders = Get-ChildItem $RepoPath
    foreach ($RepoSubfolder in $RepoSubfolders) 
    {
        $LocalSubfolderPath = Join-Path $LocalModelPath $RepoSubfolder
        $RepoSubfolderPath = Join-Path $RepoPath $RepoSubfolder

        if (Test-Path $RepoSubfolderPath -PathType Container)
        {
            # Use CMD and rmdir since Powershell Remove-Item tries to recurse subfolders
            Write-Host "Removing existing $($Model)\$($RepoSubfolder) source code"
            cmd /c rmdir /s /q $LocalSubfolderPath

            # Create new symbolic links
            Write-Host "Creating new symbolic link for $($Model)\$($RepoSubfolder)"
            New-Item -ItemType:SymbolicLink -Path:$LocalSubfolderPath -Value:$RepoSubfolderPath
        }
    }   
}
pause