$replacements = [ordered]@{
    ([char]0x00c3 + [char]0x00a1) = [char]0x00e1 # á
    ([char]0x00c3 + [char]0x00a9) = [char]0x00e9 # é
    ([char]0x00c3 + [char]0x00ad) = [char]0x00ed # í
    ([char]0x00c3 + [char]0x00b3) = [char]0x00f3 # ó
    ([char]0x00c3 + [char]0x00ba) = [char]0x00fa # ú
    ([char]0x00c3 + [char]0x00b1) = [char]0x00f1 # ñ
    ([char]0x00c3 + [char]0x201c) = [char]0x00d3 # Ó
    ([char]0x00c3 + [char]0x0161) = [char]0x00da # Ú
    ([char]0x00c2 + [char]0x00bf) = [char]0x00bf # ¿
    ([char]0x00c2 + [char]0x00a1) = [char]0x00a1 # ¡
    ([char]0x00c2 + [char]0x00a9) = [char]0x00a9 # ©
}

$files = Get-ChildItem -Path "Delivery.MVC" -Recurse -Include *.resx, *.cshtml, *.cs

foreach ($file in $files) {
    # Read as UTF8
    $content = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
    
    $changed = $false
    foreach ($key in $replacements.Keys) {
        if ($content.Contains($key)) {
            $content = $content.Replace($key, $replacements[$key])
            $changed = $true
        }
    }
    
    if ($changed) {
        Write-Host "Fixing $($file.Name)"
        [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
    }
}
