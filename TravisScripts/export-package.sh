#! /bin/sh

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

# returning license
echo "returning license"
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense

exit