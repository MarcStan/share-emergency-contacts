$vpatch = New-Object System.Uri("https://marcstan.net/binaries/live/vpatch.exe")
$icogen = New-Object System.Uri("https://marcstan.net/binaries/live/icogen.exe")
$webclient = New-Object System.Net.WebClient
$webclient.DownloadFile($vpatch, "vpatch.exe")
$webclient.DownloadFile($icogen, "icogen.exe")