#! /bin/sh

# NOTE the command args below make the assumption that your Unity project folder is
#  a subdirectory of the repo root directory, e.g. for this repo "unity-ci-test" 
#  the project folder is "UnityProject". If this is not true then adjust the 
#  -projectPath argument to point to the right location.

## Run the editor unit tests
echo "Running editor unit tests for ${UNITYCI_PROJECT_NAME} editmode"

/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-logFile $(pwd)/editor1.log \
	-projectPath $(pwd)/${UNITYCI_PROJECT_NAME} \
	-runTests \
	-testResults $(pwd)/test1.xml \
	-testPlatform editmode \
	-username ${UNITYCI_USER_NAME} \
	-password ${UNITYCI_PASS} \
	-serial ${UNITYCI_SERIAL}

rc0=$?
echo "Unit test logs"
cat $(pwd)/test1.xml
#exit if tests failed
if [ $rc0 -ne 0 ]; then { echo "Failed unit tests editmode"; /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense; exit $rc0; } fi	

echo "Running editor unit tests for ${UNITYCI_PROJECT_NAME} playmode"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-logFile $(pwd)/editor2.log \
	-projectPath $(pwd)/${UNITYCI_PROJECT_NAME} \
	-runTests \
	-testResults $(pwd)/test2.xml \
	-testPlatform playmode \
	-username ${UNITYCI_USER_NAME} \
	-password ${UNITYCI_PASS} \
	-serial ${UNITYCI_SERIAL}

rc1=$?
echo "Unit test logs 2"
cat $(pwd)/test2.xml
#exit if tests failed
if [ $rc1 -ne 0 ]; then { echo "Failed unit tests playmode"; /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense; exit $rc1; } fi	

# returning license
echo "returning license"
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense

exit