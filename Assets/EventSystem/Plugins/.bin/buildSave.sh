#!/bin/sh

pluginpath=$1

if [ ! $pluginpath ] ; then
	if [ -d "Plugins" ] ; then
		echo "AUTO-DISCOVERY"
		pluginpath="Plugins/"
	else
		echo "ERROR: You must specify the path to Plugins/ from here"
		exit 10;
	fi
fi

DLL=MTTE.dll # Meticulout Tech Texas Editor

SCRIPTS="$pluginpath/Utility/*.cs"
DIRS="$pluginpath/Utility/"

echo "Building plugins at $pluginpath"

echo gmcs -r:/Applications/Unity\ 2.5rc5/Unity.app/Contents/Frameworks/UnityEngine.dll -target:library -out:$pluginpath/$DLL $SCRIPTS
gmcs -r:/Applications/Unity\ 2.5rc5/Unity.app/Contents/Frameworks/UnityEngine.dll -target:library -out:$pluginpath/$DLL $SCRIPTS

if [ $? == 0 ] ; then
	rm -rf $SCRIPTS $DIRS
else
	echo "FAIL"
fi
