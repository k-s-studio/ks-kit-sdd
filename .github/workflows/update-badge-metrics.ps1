param(
	[string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
)

$agentsPath = Join-Path $RepoRoot ".github\agents"
$skillsPath = Join-Path $RepoRoot ".github\skills"
$promptsPath = Join-Path $RepoRoot ".github\prompts"
$outputPath = Join-Path $RepoRoot ".github\workflows\metrics.json"

function Get-FileCount {
	param(
		[string]$Path,
		[string]$Filter,
		[switch]$Recurse
	)

	if (-not (Test-Path $Path)) {
		return 0
	}

	return @(Get-ChildItem -Path $Path -File -Filter $Filter -Recurse:$Recurse).Count
}

$metrics = [ordered]@{
	agents = Get-FileCount -Path $agentsPath -Filter "*.agent.md"
	skills = Get-FileCount -Path $skillsPath -Filter "SKILL.md" -Recurse
	prompts = Get-FileCount -Path $promptsPath -Filter "*.prompt.md"
	updatedAtUtc = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
}

$json = ($metrics | ConvertTo-Json) + [Environment]::NewLine
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText($outputPath, $json, $utf8NoBom)