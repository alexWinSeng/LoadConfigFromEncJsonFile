# Import-Module C:\Users\user\Desktop\encryptModule.psm1
# Protect-AesString -filePath 'C:\Users\user\source\repos\DataProtectApi\DataProtectApi\appsettings2.json'

Function Protect-AesString 
{
  [CmdletBinding()]
  Param(
    [Parameter(Position=0, Mandatory=$true)]
    [String]$filePath = 'C:\Users\user\source\repos\DataProtectApi\DataProtectApi\appsettings1.json'
  )
  Try 
  {
	$machineName = [Environment]::MachineName
	$salt = $machineName.PadLeft(8, ' ')
	$saltBytes = [Text.Encoding]::UTF8.GetBytes($salt)
	$iterations = 50000
	$derivedBytes = New-Object Security.Cryptography.Rfc2898DeriveBytes `
		-ArgumentList @($machineName, $saltBytes, $iterations, 'SHA256')
	$aesKeyBytes = $derivedBytes.GetBytes(32)
	$derivedBytes.Dispose();
	
	$key = [System.Linq.Enumerable]::ToArray([System.Linq.Enumerable]::Take($aesKeyBytes, 32))
	$iv = [System.Linq.Enumerable]::ToArray([System.Linq.Enumerable]::Take($aesKeyBytes, 16))
	
	$cipher = [System.Security.Cryptography.Aes]::Create()
	$cipher.Key = $key
	$cipher.IV = $iv
	$encryptor = $cipher.CreateEncryptor()
	
	
	$memoryStream = New-Object -TypeName IO.MemoryStream
	$cryptoStream = New-Object -TypeName Security.Cryptography.CryptoStream `
	-ArgumentList @( $memoryStream, $encryptor, 'Write' )
	
	$jsonBytes = [System.IO.File]::ReadAllBytes($filePath)
	$cryptoStream.Write($jsonBytes, 0, $jsonBytes.Length)
	$cryptoStream.FlushFinalBlock()
	$encryptedJsonBytes = $memoryStream.ToArray()
	
	$cipher.Clear()
	$memoryStream.SetLength(0)
	$memoryStream.Close()
	$cryptoStream.Clear()
	$cryptoStream.Close()
	
	# Base64 Encode the encrypted bytes to get a string
	$encryptedJsonBase64 = [Convert]::ToBase64String($encryptedJsonBytes)
	
	# https://stackoverflow.com/questions/44597175/creating-a-json-string-powershell-object
	$jsonRequest = [ordered]@{
		Content = $encryptedJsonBase64
	}
	
	$jsonFileContent = $jsonRequest | ConvertTo-Json -Depth 10
	
	$jsonFileContentBytes = [Text.Encoding]::UTF8.GetBytes($jsonFileContent)
	[System.IO.File]::WriteAllBytes($filePath, $jsonFileContentBytes)
  }
  Catch
  {
    Write-Error $_
  }
}