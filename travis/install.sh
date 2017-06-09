#!/bin/sh

echo 'Downloading Unity 5.6.1f1: '
wget -O Unity.pkg http://netstorage.unity3d.com/unity/2860b30f0b54/MacEditorInstaller/Unity-5.6.1f1.pkg
wget -O Unity-Win.pkg http://netstorage.unity3d.com/unity/2860b30f0b54/MacEditorTargetInstaller/UnitySetup-Windows-Support-for-Editor-5.6.1f1.pkg
echo 'Installing Unity.pkg'
sudo installer -dumplog -package Unity.pkg -target /
sudo installer -dumplog -package Unity-Win.pkg -target /
