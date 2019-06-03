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
	-projectPath "$(pwd)/${UNITYCI_PROJECT_NAME}" \
	-runTests \
	-testResults $(pwd)/test1.xml \
	-testPlatform editmode \
	-username "${UNITYCI_NEW_USER}" \
	-password "${UNITYCI_NEW_PASS}" \
	-serial "${UNITYCI_NEW_SERIAL}" 
	# -silent-crashes \
	# -accept-apiupdate \
	# -noUpm

rc0=$?
echo "Unit test logs"
#cat $(pwd)/editor1.log
cat $(pwd)/test1.xml

# returning license
echo "returning license"
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense

#exit if tests failed
#if [ $rc0 -ne 0 ]; then { echo "Failed unit tests editmode"; /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense; exit $rc0; } fi	
if [ $rc1 -ne 0 ]; then { echo "Failed unit tests editmode"; exit $rc1; } fi	

echo "Running editor unit tests for ${UNITYCI_PROJECT_NAME} playmode"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-logFile $(pwd)/editor2.log \
	-projectPath "$(pwd)/${UNITYCI_PROJECT_NAME}" \
	-runTests \
	-testResults $(pwd)/test2.xml \
	-testPlatform playmode \
	-username "${UNITYCI_NEW_USER}" \
	-password "${UNITYCI_NEW_PASS}" \
	-serial "${UNITYCI_NEW_SERIAL}" 
	# -silent-crashes \
	# -nographics \
	# -noUpm	

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

echo "creating exportPackage"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-quit \
	-batchmode \
	-logFile $(pwd)/exportPackage.log \
	-projectPath $(pwd)/${UNITYCI_PROJECT_NAME} \
	-exportPackage "Assets" "${UNITYCI_PACKAGE_NAME}.unitypackage" \
	-username ${UNITYCI_USER_NAME} \
	-password ${UNITYCI_PASS} \
	-serial ${UNITYCI_SERIAL} \
	# -nographics

# returning license
echo "returning license"
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense

exit