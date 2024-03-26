echo "Compressing artifacts into one file" 
powershell Compress-Archive -Path "C:\ProgramData\Jenkins\.jenkins\workspace\TestEaseBuild\TestEase\TestEase\bin\x64\Release\net8.0-windows10.0.19041.0\win10-x64\publish\*" -DestinationPath publish.zip

echo "Creating a new release in github" 
set GITHUB_TOKEN=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 
set GITHUB_API=https://github.ncsu.edu/api/v3/
set REPO=2024SpringTeam31-Hitachi-2
set ORG=engr-csc-sdc
set TAG=v0
set NAME="Release Name"

curl -X POST -H "Authorization: token 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 " -d "{ \"tag_name\": \"v0\", \"target_commitish\": \"jenkins-publish\", \"name\": \"Release Name\", \"body\": \"Description of the release\", \"draft\": false, \"prerelease\": false }" https://github.ncsu.edu/api/v3/repos/engr-csc-sdc/2024SpringTeam31-Hitachi-2/releases
echo "Uploading the artifacts into github" 
set FILE=publish.zip

curl -X POST -H "Authorization: token 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 " -H "Content-Type: application/zip" --data-binary @publish.zip https://github.ncsu.edu/api/v3/repos/engr-csc-sdc/2024SpringTeam31-Hitachi-2/releases/v0/assets?name=publish.zip 