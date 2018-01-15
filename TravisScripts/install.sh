#! /bin/sh

# Downloads
echo 'Downloading Unity-2017.3.0f3 pkg:'
#curl --retry 5 -o Unity.pkg https://netstorage.unity3d.com/unity/46dda1414e51/MacEditorInstaller/Unity-2017.2.0f3.pkg
curl --retry 5 -o Unity.pkg https://netstorage.unity3d.com/unity/a9f86dcd79df/MacEditorInstaller/Unity-2017.3.0f3.pkg
if [ $? -ne 0 ]; then { echo "Unity Download failed"; exit $?; } fi

#echo 'Downloading iOS build support:'
#curl --retry 5 -o Unity_iOS.pkg https://beta.unity3d.com/download/46dda1414e51/MacEditorTargetInstaller/UnitySetup-iOS-Support-for-Editor-2017.2.0f3.pkg
#if [ $? -ne 0 ]; then { echo "iOS Download failed"; exit $?; } fi

#echo 'Downloading Android build support:'
#curl --retry 5 -o Unity_Android.pkg https://beta.unity3d.com/download/46dda1414e51/MacEditorTargetInstaller/UnitySetup-Android-Support-for-Editor-2017.2.0f3.pkg
#if [ $? -ne 0 ]; then { echo "Download failed"; exit $?; } fi

#echo 'Downloading WebGL build support:'
#curl --retry 5 -o Unity_WebGL.pkg https://beta.unity3d.com/download/46dda1414e51/MacEditorTargetInstaller/UnitySetup-WebGL-Support-for-Editor-2017.2.0f3.pkg
#if [ $? -ne 0 ]; then { echo "Download failed"; exit $?; } fi

#echo 'Downloading Windows build support:'
#curl --retry 5 -o Unity_Win.pkg https://beta.unity3d.com/download/46dda1414e51/MacEditorTargetInstaller/UnitySetup-Windows-Support-for-Editor-2017.2.0f3.pkg
#if [ $? -ne 0 ]; then { echo "Download failed"; exit $?; } fi

#echo 'Downloading Linux build support:'
#curl --retry 5 -o Unity_Linux.pkg https://beta.unity3d.com/download/46dda1414e51/MacEditorTargetInstaller/UnitySetup-Linux-Support-for-Editor-2017.2.0f3.pkg
#if [ $? -ne 0 ]; then { echo "Download failed"; exit $?; } fi

# Install
echo 'Installing Unity.pkg'
sudo installer -dumplog -package Unity.pkg -target /

echo "Verify firewall"
/usr/libexec/ApplicationFirewall/socketfilterfw --getappblocked /Applications/Unity/Unity.app/Contents/MacOS/Unity

echo "Create Certificate Folder"
mkdir ~/Library/Unity
mkdir ~/Library/Unity/Certificates

cp "./TravisScripts/CACerts.pem" ~/Library/Unity/Certificates/

#echo "activate license"
#/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -serial ${UNITYCI_SERIAL} -username ${UNITYCI_USER_NAME} -password ${UNITYCI_PASS} -logfile
#/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -username ${UNITYCI_USER_NAME} -password ${UNITYCI_PASS} -logfile

#cat ~/Library/Logs/Unity/Editor.log

#echo "return license"

#/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -returnlicense -logfile

echo === Done ===

#echo 'Installing Unity_iOS.pkg'
#sudo installer -dumplog -package Unity_iOS.pkg -target /
#echo 'Installing Unity_Android.pkg'
#sudo installer -dumplog -package Unity_Android.pkg -target /
#echo 'Installing Unity_WebGL.pkg'
#sudo installer -dumplog -package Unity_WebGL.pkg -target /
#echo 'Installing Unity_win.pkg'
#sudo installer -dumplog -package Unity_Win.pkg -target /
#echo 'Installing Unity_Linux.pkg'
#sudo installer -dumplog -package Unity_Linux.pkg -target /
