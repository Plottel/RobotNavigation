echo Which assets are you adding?
read assetDescription

commitMessage="ASSETS: $assetDescription"

git add .
git commit -am "$commitMessage"
git push -u origin master

echo "$commitMessage"

read keepScriptOpen