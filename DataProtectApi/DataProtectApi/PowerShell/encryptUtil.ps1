# Powershell.exe -executionpolicy remotesigned -File C:\Users\user\Desktop\encryptUtil.ps1 -encryptModuleFilePath 'C:\Users\user\Desktop\encryptModule.psm1' -filePath 'C:\Users\user\source\repos\DataProtectApi\DataProtectApi\' -fileNames 'appsettings2.json,appsettings3.json'
param(
    [Parameter(Position=0, Mandatory=$True, ValueFromPipeline=$false)]
    [System.String]$encryptModuleFilePath,

    [Parameter(Position=1, Mandatory=$True, ValueFromPipeline=$false)]
    [System.String]$filePath,

    [Parameter(Position=2, Mandatory=$True, ValueFromPipeline=$false)]
    [System.String]$fileNames
)
Import-Module $encryptModuleFilePath
$option = [System.StringSplitOptions]::RemoveEmptyEntries
$fileNameList = $fileNames.Split(',', $option)
foreach($fileName in $fileNameList)
{
	$fileFullPath = [System.IO.Path]::Combine($filePath, $fileName);
	# echo $fileFullPath
	Protect-AesString -filePath $fileFullPath
}