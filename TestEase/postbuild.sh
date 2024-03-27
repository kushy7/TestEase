echo "Compressing artifacts into one file"
powershell Compress-Archive -Path "C:\ProgramData\Jenkins\.jenkins\workspace\TestEaseBuild\TestEase\TestEase\bin\x64\Release\net8.0-windows10.0.19041.0\win10-x64\publish\*" -DestinationPath publish.zip

echo "Creating a new release in GitHub"
set GITHUB_TOKEN=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
set GITHUB_API=https://github.ncsu.edu/api/v3/
set REPO=2024SpringTeam31-Hitachi-2
set ORG=engr-csc-sdc
set TAG=v0
set NAME="Release Name"

echo Create the release and capture the release ID
for /f "tokens=*" %%a in ('curl -X POST -H "Authorization: token %GITHUB_TOKEN%" -d "{\"tag_name\": \"%TAG%\", \"target_commitish\": \"jenkins-publish\", \"name\": \"%NAME%\", \"body\": \"Description of the release\", \"draft\": false, \"prerelease\": false }" "%GITHUB_API%repos/%ORG%/%REPO%/releases"') do (set RELEASE_RESPONSE=%%a)

echo Parse the release ID from the response
for /f "tokens=2 delims=:" %%a in ("%RELEASE_RESPONSE%") do (set RELEASE_ID=%%a)

echo Release created with ID: %RELEASE_ID%

echo "Uploading the artifacts into GitHub"
set FILE=publish.zip

echo Use the obtained release ID to upload the artifact
curl -X POST -H "Authorization: token %GITHUB_TOKEN%" -H "Content-Type: application/zip" --data-binary @%FILE% "%GITHUB_API%repos/%ORG%/%REPO%/releases/%RELEASE_ID%/assets?name=%FILE%"
