echo "Compressing artifacts into one file"
zip -r publish.zip ./publish

echo "Exporting token and enterprise api to enable github-release tool"
export GITHUB_TOKEN=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
export GITHUB_API=https://api.github.com # needed only for enterprise

echo "Deleting release from github before creating new one"
github-release delete --user "ejsamuel" --repo "2024SpringTeam31-Hitachi-2" --tag "v0"	

echo "Creating a new release in github"
github-release release --user "ejsamuel" --repo "2024SpringTeam31-Hitachi-2" --tag "v0" --name "Release Name"

echo "Uploading the artifacts into github"
github-release upload --user "ejsamuel" --repo "2024SpringTeam31-Hitachi-" --tag "v0" --name "publish.zip" --file publish.zip
