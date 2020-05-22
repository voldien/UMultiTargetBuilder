#!/bin/sh

$unityPath=$1 # First argument - Path to the Unity executable. 
$path=$2      # Second argument - Path to the project root direcrory.
$fast=$3      # Script build or full build. ( argument s for script build and argument f for full build)

if [ $fast == 'f' ]
then
    $unityPath -projectPath "$path" -batchmode -nographics -executeMethod BuildMultiPlatform.Builder.PerformBuildContext -quit
elif [ $fast == 's' ]
    $unityPath -projectPath "$path" -batchmode -nographics -executeMethod BuildMultiPlatform.Builder.PerformBuildScriptOnlyContext -quit
fi
