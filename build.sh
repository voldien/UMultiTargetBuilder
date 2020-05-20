#!/bin/sh

$unityPath=$1 # First argument - Path to the Unity executable. 
$path=$2      # Second argument - Path to the project root direcrory.
$unityPath -projectPath "$path" -batchmode -nographics -executeMethod Builder.PerformBuildContext -quit


#$unityPath -projectPath "$path" -batchmode -nographics -executeMethod Builder.PerformBuildScriptOnlyContext -quit

