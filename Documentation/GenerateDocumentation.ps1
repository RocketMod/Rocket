$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath
cd $dir

Invoke-WebRequest "http://www.naturaldocs.org/download/natural_docs/2.0.1/Natural_Docs_2.0.1.zip" -OutFile "Natural_Docs.zip"
New-Item -ItemType directory -Path ".\HTML\"
Expand-Archive -Path "Natural_Docs.zip" -DestinationPath .\ -Force 
Remove-Item "Natural_Docs.zip"
& ".\Natural Docs\NaturalDocs.exe" ".\"
