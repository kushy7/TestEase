echo "Compressing artifacts into one file" 
powershell Compress-Archive -Path ./publish/* -DestinationPath publish.zip

echo "Exporting token and enterprise api to enable github-release tool" 
set GITHUB_TOKEN=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 
set GITHUB_API=https://github.ncsu.edu/api/v3/

echo "Deleting release from github before creating new one" 
github-release delete --user "ejsamuel" --repo "2024SpringTeam31-Hitachi-2" --tag "v0"

echo "Creating a new release in github" 
github-release release --user "ejsamuel" --repo "2024SpringTeam31-Hitachi-2" --tag "v0" --name "Release Name"

echo "Uploading the artifacts into github" 
github-release upload --user "ejsamuel" --repo "2024SpringTeam31-Hitachi-" --tag "v0" --name "publish.zip" --file publish.zip
