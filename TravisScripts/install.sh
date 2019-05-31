#! /bin/sh

# Downloads
echo 'Downloading Unity pkg:'

# curl -v http://license.unity3d.com/ > $(pwd)/license.txt
# cat $(pwd)/license.txt

# curl -v http://activation.unity3d.com/ > $(pwd)/activation.txt
# cat $(pwd)/activation.txt

# curl -v http://sl-http.unity3d.com/ > $(pwd)/sl-http.txt
# cat $(pwd)/sl-http.txt

# curl -v http://developer.cloud.unity3d.com/ > $(pwd)/developer.txt
# cat $(pwd)/developer.txt

# curl -v http://core.cloud.unity3d.com/ > $(pwd)/core.txt
# cat $(pwd)/core.txt

# curl -v http://accounts.unity3d.com > $(pwd)/accounts.txt
# cat $(pwd)/accounts.txt

#2019.1.4
#curl --retry 5 -o Unity.pkg https://download.unity3d.com/download_unity/ffa3a7a2dd7d/MacEditorInstaller/Unity.pkg
#2018.2.21
curl --retry 5 -o Unity.pkg https://download.unity3d.com/download_unity/a122f5dc316d/MacEditorInstaller/Unity.pkg
if [ $? -ne 0 ]; then { echo "Unity Download failed"; exit $?; } fi

#echo 'Downloading StandardAssets pkg:'
#curl --retry 5 -o Unity_StandardAssets.pkg https://download.unity3d.com/download_unity/46dda1414e51/MacStandardAssetsInstaller/StandardAssets-2017.2.0f3.pkg
#curl --retry 5 -o Unity_StandardAssets.pkg https://download.unity3d.com/download_unity/24bbd83e8b9e/MacStandardAssetsInstaller/StandardAssets-2018.1.9f1.pkg
#if [ $? -ne 0 ]; then { echo "Unity StandardAssets Download failed"; exit $?; } fi

# Install
echo 'Installing Unity pkg'
sudo installer -dumplog -package Unity.pkg -target /

echo "Verify firewall"
/usr/libexec/ApplicationFirewall/socketfilterfw --getappblocked /Applications/Unity/Unity.app/Contents/MacOS/Unity

echo "Create Certificate Folder"
mkdir ~/Library/Unity
mkdir ~/Library/Unity/Certificates

cp "./TravisScripts/CACerts.pem" ~/Library/Unity/Certificates/
cp "./TravisScripts/rest-certificate.pem" ~/Library/Unity/


#echo === Done ===
#echo 'Installing StandardAssets pkg'
#sudo installer -dumplog -package Unity_StandardAssets.pkg -target /


