properties {
	$basedir = Resolve-Path .
	$solution = "$basedir\DeployTestApp.sln"
}

Task default -depends Compile, Publish

Task Compile {
	msbuild "$solution" /p:"RunOctoPack=true" /p:"OctoPackPublishPackageToFileShare=`".\..\build`""
}

Task Publish {
    cp "build\*.nupkg" "d:\dev\virtuals\share\packages"
}