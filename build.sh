#!/bin/bash
#
# SCRIPT: Building Unity targets using the MultiTarget Builder tool.
# AUTHOR: Valdemar Lindberg
# DATE: 25/05/2020
# REV: 1.0
#
# PLATFORM: Not platform dependent.
#
# PURPOSE:# Creating Unity builds from the command line without
# using the GUI interface.
#
# set -n
# Uncomment to check script syntax, without execution.
#
# set -x
# Uncomment to debug this shell script
# 
# Example: ./build.sh /path/unity/Editor/Unity /path/project-directory -f
# This will build a project with the 
#

unityPath=$1   # First argument - Path to the Unity executable. 
path=$2        # Second argument - Path to the project root direcrory.
buildmode=$3   # Script build or full build. ( argument s for script build and argument f for full build)
additional=${4:-""}

if [ $buildmode == 'f' ]
then
    $unityPath -projectPath "$path" -batchmode -nographics -executeMethod BuildMultiPlatform.Builder.PerformBuildContext -quit $additional
elif [ $fasbuildmodet == 's' ] 
then
    $unityPath -projectPath "$path" -batchmode -nographics -executeMethod BuildMultiPlatform.Builder.PerformBuildScriptOnlyContext -quit $additional
fi
