#!/bin/bash
# $1 is TargetName, $2 is TargetPath, $3 is SolutionDir
GAME_DIR="/home/osema/.steam/debian-installation/steamapps/common/Subnautica"

mkdir -p "$GAME_DIR/BepInEx/plugins/$1"
cp -f "$2" "$GAME_DIR/BepInEx/plugins/$1"

mkdir -p "$3/Release/$1/BepInEx/plugins/$1"
cp -f "$2" "$3/Release/$1/BepInEx/plugins/$1/"
