$testResultsFile = "C:\temp\results.xml"
Start-Process -Wait -FilePath Unity.exe -ArgumentList ('-batchmode', '-nographics', '-logFile', 'C:\temp\unity.log', '-runTests', '-projectPath', 'c:\projects\unity\PubNubUnity', '-testResults', $testResultsFile, '-testPlatform', 'editmode', '-quit')
$res = Invoke-Pester -OutputFormat NUnitXml -OutputFile $testResultsFile -PassThru
(New-Object 'System.Net.WebClient').UploadFile("https://ci.appveyor.com/api/testresults/nunit/$($env:APPVEYOR_JOB_ID)", (Resolve-Path $testResultsFile))
if ($res.FailedCount -gt 0) { 
    throw "$($res.FailedCount) tests failed."
}  

$testResultsFile2 = "C:\temp\results2.xml"
Start-Process -Wait -FilePath Unity.exe -ArgumentList ('-batchmode', '-nographics', '-logFile', 'C:\temp\unity.log', '-runTests', '-projectPath', 'c:\projects\unity\PubNubUnity', '-testResults', $testResultsFile2, '-testPlatform', 'playmode', '-quit')
$res2 = Invoke-Pester -OutputFormat NUnitXml -OutputFile $testResultsFile2 -PassThru
(New-Object 'System.Net.WebClient').UploadFile("https://ci.appveyor.com/api/testresults/nunit/$($env:APPVEYOR_JOB_ID)", (Resolve-Path $testResultsFile2))
if ($res2.FailedCount -gt 0) { 
    throw "$($res2.FailedCount) tests failed."
}  
