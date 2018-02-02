$testResultsFile = "C:\temp\results.xml"
Start-Process -Wait -FilePath Unity.exe -ArgumentList ('-batchmode', '-nographics', '-logFile', 'C:\temp\unity.log', '-runTests', '-projectPath', 'c:\projects\unity\PubNubUnity', '-testResults', $testResultsFile, '-testPlatform', 'editmode', '-quit')
$res = Invoke-Pester -OutputFormat NUnitXml -OutputFile $testResultsFile -PassThru
(New-Object 'System.Net.WebClient').UploadFile("https://ci.appveyor.com/api/testresults/nunit/$($env:APPVEYOR_JOB_ID)", (Resolve-Path $testResultsFile))
if ($res.FailedCount -gt 0) { 
    throw "$($res.FailedCount) tests failed."
}  
