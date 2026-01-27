$content = Get-Content "NuklearDotNetDotnet.sln"
$newContent = @()
$raylibLines = @()
$skipNext = $false

for ($i = 0; $i -lt $content.Length; $i++) {
    $line = $content[$i]
    
    # Check if this is the Raylib project line
    if ($line -match 'Example_Raylib.*Example_RaylibDotnet\.csproj') {
        $raylibLines += $line
        $skipNext = $true
        continue
    }
    
    # Skip the EndProject after Raylib
    if ($skipNext -and $line -eq "EndProject") {
        $raylibLines += $line
        $skipNext = $false
        continue
    }
    
    # Insert Raylib before Example_SFML
    if ($line -match 'Example_SFML.*Example_SFMLDotnet\.csproj') {
        foreach ($rl in $raylibLines) {
            $newContent += $rl
        }
    }
    
    $newContent += $line
}

$newContent | Set-Content "NuklearDotNetDotnet.sln"
Write-Host "Solution reordered - Example_Raylib is now the first example project"
