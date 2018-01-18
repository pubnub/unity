#! /bin/sh

# NOTE the command args below make the assumption that your Unity project folder is
#  a subdirectory of the repo root directory, e.g. for this repo "unity-ci-test" 
#  the project folder is "UnityProject". If this is not true then adjust the 
#  -projectPath argument to point to the right location.

## Run the editor unit tests
echo "Running editor unit tests for ${UNITYCI_PROJECT_NAME} StandaloneOSXUniversal"
echo "Test ${UNITYCI_TEST}"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
 	-batchmode \
 	-logFile $(pwd)/unity.log \
 	-projectPath "$(pwd)/${UNITYCI_PROJECT_NAME}" \
 	-runTests \
 	-testResults $(pwd)/test.xml \
 	-testPlatform StandaloneOSXUniversal \
 	-username ${UNITYCI_USER_NAME} \
 	-password ${UNITYCI_PASS} \
 	-serial ${UNITYCI_SERIAL} 

rc0=$?
echo "Unity Logs:"
cat ~/Library/Logs/Unity/Editor.log
cat $(pwd)/unity.log
echo "Unit test logs"
cat $(pwd)/test.xml
exit if tests failed
if [ $rc0 -ne 0 ]; then { echo "Failed unit tests"; /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense; exit $rc0; } fi	

#/Applications/Unity/Unity.app/Contents/MacOS/Unity \
#	-batchmode \
#	-logFile $(pwd)/unity.log \
#	-projectPath "$(pwd)/${UNITYCI_PROJECT_NAME}" \
#	-runTests \
#	-testResults $(pwd)/test2.xml \
#	-testPlatform playmode \
#	-username ${UNITYCI_USER_NAME} \
#	-password ${UNITYCI_PASS} \
#	-serial ${UNITYCI_SERIAL} 

#rc3=$?
#echo "Unity Logs:"
#cat ~/Library/Logs/Unity/Editor.log
#cat $(pwd)/unity.log
#echo "Unit test logs"
#cat $(pwd)/test2.xml
# exit if tests failed
#if [ $rc3 -ne 0 ]; then { echo "Failed unit tests"; /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense; exit $rc0; } fi	


## Make the builds
# echo "Attempting build of ${UNITYCI_PROJECT_NAME} for Windows"
# /Applications/Unity/Unity.app/Contents/MacOS/Unity \
# 	-batchmode \
# 	-nographics \
# 	-silent-crashes \
# 	-logFile $(pwd)/unity.log \
# 	-projectPath "$(pwd)/${UNITYCI_PROJECT_NAME}" \
# 	-executeMethod "AutoBuilder.PerformStandaloneWindows64" \
# 	-quit
	
# 	#-buildWindowsPlayer "$(pwd)/Build/windows/${UNITYCI_PROJECT_NAME}.exe" \
	

#rc1=$?
# echo "Build logs (Windows)"
#cat $(pwd)/unity.log

# echo "Attempting build of ${UNITYCI_PROJECT_NAME} for OSX"
# /Applications/Unity/Unity.app/Contents/MacOS/Unity \
# 	-batchmode \
# 	-nographics \
# 	-silent-crashes \
# 	-logFile $(pwd)/unity.log \
# 	-projectPath "$(pwd)/${UNITYCI_PROJECT_NAME}" \
# 	-executeMethod "AutoBuilder.PerformStandaloneOSXUniversal" \
# 	-quit
	
	#-buildOSXUniversalPlayer "$(pwd)/Build/osx/${UNITYCI_PROJECT_NAME}.app" \
	
#rc2=$?
#echo "Build logs (OSX)"
#cat $(pwd)/unity.log

# returning license
echo "returning license"
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense

#exit $(($rc1|$rc2))
exit