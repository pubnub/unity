#! /bin/sh

# Downloads
echo 'Downloading Unity pkg:'
curl --retry 5 -o Unity.pkg https://download.unity3d.com/download_unity/24bbd83e8b9e/MacEditorInstaller/Unity-2018.1.9f1.pkg
if [ $? -ne 0 ]; then { echo "Unity Download failed"; exit $?; } fi

echo 'Downloading StandardAssets pkg:'
#curl --retry 5 -o Unity_StandardAssets.pkg https://download.unity3d.com/download_unity/46dda1414e51/MacStandardAssetsInstaller/StandardAssets-2017.2.0f3.pkg
curl --retry 5 -o Unity_StandardAssets.pkg https://download.unity3d.com/download_unity/24bbd83e8b9e/MacStandardAssetsInstaller/StandardAssets-2018.1.9f1.pkg
if [ $? -ne 0 ]; then { echo "Unity StandardAssets Download failed"; exit $?; } fi

# Install
echo 'Installing Unity pkg'
sudo installer -dumplog -package Unity.pkg -target /

echo "Verify firewall"
/usr/libexec/ApplicationFirewall/socketfilterfw --getappblocked /Applications/Unity/Unity.app/Contents/MacOS/Unity

echo "Create Certificate Folder"
mkdir ~/Library/Unity
mkdir ~/Library/Unity/Certificates

cp "./TravisScripts/CACerts.pem" ~/Library/Unity/Certificates/

echo === Done ===
echo 'Installing StandardAssets pkg'
sudo installer -dumplog -package Unity_StandardAssets.pkg -target /


