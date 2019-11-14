#! /bin/sh

echo "Running editor unit tests for ${UNITYCI_PROJECT_NAME} playmode"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-logFile $(pwd)/editor2.log \
	-projectPath "$(pwd)/${UNITYCI_PROJECT_NAME}" \
	-runEditorTests \
	-testResults $(pwd)/test2.xml \
	-testPlatform playmode \
	-username "${UNITYCI_NEW_USER}" \
	-password "${UNITYCI_NEW_PASS}" \
	-serial "${UNITYCI_NEW_SERIAL}" 

rc1=$?
echo "Unit test logs 2"
#cat $(pwd)/editor2.log
cat $(pwd)/test2.xml

# returning license
echo "returning license"
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense

#exit if tests failed
#if [ $rc1 -ne 0 ]; then { echo "Failed unit tests playmode"; /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense; exit $rc1; } fi	
if [ $rc1 -ne 0 ]; then { echo "Failed unit tests playmode"; exit $rc1; } fi	
