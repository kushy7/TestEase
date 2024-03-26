echo "Compressing artifacts into one file" 
powershell Compress-Archive -Path "C:\ProgramData\Jenkins\.jenkins\workspace\TestEaseBuild\TestEase\TestEase\bin\x64\Release\net8.0-windows10.0.19041.0\win10-x64\publish\*" -DestinationPath publish.zip

echo "Creating a new release in github" 
set GITHUB_TOKEN=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 
set GITHUB_API=https://github.ncsu.edu/api/v3/
set REPO=2024SpringTeam31-Hitachi-2
set USER=ejsamuel
set TAG=v0
set NAME="Release Name"

curl -X POST -H "Authorization: token %GITHUB_TOKEN%" -d "{ \"tag_name\": \"%TAG%\", \"target_commitish\": \"jenkins-publish\", \"name\": \"%NAME%\", \"body\": \"Description of the release\", \"draft\": false, \"prerelease\": false }" %GITHUB_API%/repos/%USER%/%REPO%/releases

echo "Uploading the artifacts into github" 
set FILE=publish.zip

curl -X POST -H "Authorization: token %GITHUB_TOKEN%" -H "Content-Type: $(file -b --mime-type %FILE%)" --data-binary @%FILE% %GITHUB_API%/repos/%USER%/%REPO%/releases/%TAG%/assets?name=$(basename %FILE%)


