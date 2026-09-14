#!/usr/bin/env sh
set -eu
cd "$(dirname "$0")/.."
export DOTNET_CLI_HOME="${DOTNET_CLI_HOME:-/tmp/lumen-dotnet}"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
dotnet build Tests/LumenRush.Checks.csproj --nologo
# This workspace has SDK 6.0.400, reference pack 6.0.9 and runtime 6.0.8.
# Choose the installed 6.0 runtime explicitly; no network packages are needed.
lumen_runtime=$(dotnet --list-runtimes | awk '$1 == "Microsoft.NETCore.App" && $2 ~ /^6[.]0[.]/ {print $2}' | sort -V | tail -n 1)
if [ -z "$lumen_runtime" ]; then echo "Install .NET 6 SDK/runtime to run these checks." >&2; exit 1; fi
dotnet exec --fx-version "$lumen_runtime" Tests/bin/Debug/net6.0/LumenRush.Checks.dll
