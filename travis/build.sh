#!/bin/sh

echo "start build"

/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-nographics \
	-silent-crashes \
	-logFile \
	-projectpath $(pwd)/beansjam_unity \
	-buildOSXUniversalPlayer "$(pwd)/build/mac/DonUniverso.app" \
	-quit

/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-nographics \
	-silent-crashes \
	-logFile \
	-projectpath $(pwd)/beansjam_unity \
	-buildWindowsPlayer "$(pwd)/build/win/DonUniverso.exe" \
	-quit

(cd build/mac/; zip -r ../../DonUniversoMac.zip DonUniverso.app)
(cd build/win/; zip -r ../../DonUniversoWin.zip .)
