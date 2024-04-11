echo "Compressing artifacts into one file"
powershell Compress-Archive -Path "C:\ProgramData\Jenkins\.jenkins\workspace\TestEaseBuild\TestEase\TestEase\bin\x64\Release\net8.0-windows10.0.19041.0\win10-x64\publish\*" -DestinationPath publish.zip

echo "Creating a new release in GitHub"
set GITHUB_TOKEN=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
set GITHUB_API=https://github.ncsu.edu/api/v3/
set GITHUB_UPLOAD_API=https://github.ncsu.edu/api/
set REPO=2024SpringTeam31-Hitachi-2
set ORG=engr-csc-sdc
set TAG=v3
set NAME=TestEasev3

echo Create the release and capture the release ID
for /F "tokens=*" %%a in ('curl -s -X POST -H "Authorization: token %GITHUB_TOKEN%" -d "{\"tag_name\": \"%TAG%\", \"target_commitish\": \"jenkins-publish\", \"name\": \"%NAME%\", \"body\": \"Description of the release\", \"draft\": false, \"prerelease\": false }" "%GITHUB_API%repos/%ORG%/%REPO%/releases" ^| jq -r ".id"') do (
	set RELEASE_ID=%%a
	goto :break
)
:break

echo Release created with ID: %RELEASE_ID%
echo "Uploading the artifact to GitHub"

set FILE=publish.zip

echo Use the obtained release ID: %RELEASE_I% to upload the artifact
curl -X POST -H "Authorization: token %GITHUB_TOKEN%" -H "Content-Type: application/zip" --data-binary @publish.zip "%GITHUB_UPLOAD_API%uploads/repos/%ORG%/%REPO%/releases/%RELEASE_ID%/assets?name=publish.zip"