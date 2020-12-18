version='1.0.$GITHUB_RUN_NUMBER'

echo 'create build container'
buildcon=$(buildah from mcr.microsoft.com/dotnet/sdk:5.0-buster-slim)
echo 'set workdir'
buildah config --workingdir /src $buildcon
echo 'copy proj'
buildah copy $buildcon ./SimpleBlog.csproj ./
echo 'dotnet restore'
buildah run $buildcon dotnet restore ./SimpleBlog.csproj
echo 'copy *.*'
buildah copy $buildcon ./ ./
echo 'dotnet build'
buildah run $buildcon dotnet build ./SimpleBlog.csproj -c Release -o /app/build
echo 'dotnet publish'
buildah run $buildcon dotnet publish ./SimpleBlog.csproj -c Release -o /app/publish
echo 'mount build container'
buildconmount=$(buildah mount $buildcon)
echo $buildconmount

echo 'create final container'
finalcon=$(buildah from mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim)
echo 'set workdir'
buildah config --workingdir /app $finalcon
echo 'set port'
buildah config --port 80 $finalcon
echo 'mount final container'
finalconmount=$(buildah mount $finalcon)
echo 

echo 'copy files from build to final container'
cp -r $buildconmount/app/publish/ $finalconmount/app/

echo 'config entrypoint'
buildah config --entrypoint 'dotnet SimpleBlog.dll' $finalcon

echo 'commit image'
buildah commit --format=docker $finalcon anmalkov/simple-blog:$version

echo 'push to docker hub'
buildah push --creds $DOCKERHUB_USERNAME:$DOCKERHUB_TOKEN anmalkov/simple-blog:$version docker://registry.hub.docker.com/anmalkov/simple-blog:$version
buildah push --creds $DOCKERHUB_USERNAME:$DOCKERHUB_TOKEN anmalkov/simple-blog:$version docker://registry.hub.docker.com/anmalkov/simple-blog:latest


echo 'umount everything'
buildah umount --all

echo 'delete all containers'
buildah rm --all
