#!/bin/sh

case "$1" in

 'clean')
    dotnet bin/prebuild.dll /file prebuildCoreLib.xml /clean

  ;;


  'autoclean')

    echo y|dotnet bin/prebuild.dll /file prebuildCoreLib.xml /clean

  ;;



  *)

    cp bin/System.Drawing.Common.dll.linux bin/System.Drawing.Common.dll
    dotnet bin/prebuild.dll /target vs2022 /targetframework net8_0 /excludedir = "obj | bin" /file prebuildCoreLib.xml
    echo "dotnet build -c Release OpenMetaverse.sln" > compile.sh
    chmod +x compile.sh

  ;;

esac