# execute using: powershell -ExecutionPolicy Bypass -File build-win.ps1

try{
    # make the build folder
    $outDir = "$(Get-Location)\build\"

    $cmdline = [string]::Join(" ",$args);
    Write-Output "Will pass arguments: $($cmdline)";

    # get the editor version to use
    $editor = Get-Content -Path ..\ProjectSettings\ProjectVersion.txt -TotalCount 1
    $editor = $editor -split '\s+'
    $editor = $editor[1]

    New-Item -Path "buildout.txt" -ErrorAction SilentlyContinue

    # run unity
    & "C:\Program Files\Unity\Hub\Editor\$($editor)\Editor\Unity.exe" -quit -batchmode -executeMethod GenBuild.CliBuild -logFile buildout.txt -outDir $outDir $cmdline

    # tail the output file in a new cmd window
    Get-Content -path 'buildout.txt' -Wait
}
finally{
    Write-Output "Stopping Unity if it is running..."

    # stop unity if it is running
    TASKKILL /t /f /fi Unity.exe

    # remove the build output log (it is preserved in the cmd window)
    rm buildout.txt
}