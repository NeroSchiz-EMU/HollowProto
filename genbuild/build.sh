# to pass additional arguments (like -skipbake), simply pass them when running the script
# like this: ./build-mac.sh -skipbake -expires 5


echo Will pass additional flags: $@

# get the unity verison to use by reading ProjectVersion.txt
read -r editor < ../ProjectSettings/ProjectVersion.txt
editor=`echo $editor | cut -d' ' -f2`
echo Project Unity Version: $editor

# search in the 2 default directories
if test -d "/Applications/$editor"; then
    editor="/Applications/$editor"
    
elif test -d "/Applications/Unity/Hub/Editor/$editor"; then
    editor="/Applications/Unity/Hub/Editor/$editor"
    
else
    # not found? exit
    echo Unity version $editor not found
    exit 1
fi

# run Unity and create the build
$editor/Unity.app/Contents/MacOS/Unity -quit -batchmode -executeMethod GenBuild.CliBuild -logFile /dev/stdout -outDir `pwd`/build $@
