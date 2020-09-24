# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$rootFolder = Join-Path $packFolder "../"

# List of solutions
$solutions = "./"

# List of projects
$projects = (
    "modules/Abpluz.Abp.Http.Client.SwitchableIdentityClients",
    "modules/Abpluz.Abp.LocalizableContent",
    "modules/Abpluz.Abp.LocalizableContent.EntityFrameworkCore"
)
