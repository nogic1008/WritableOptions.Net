# See here for image contents: https://github.com/devcontainers/images/tree/main/src/dotnet
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:0-7.0

# Install mono
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends mono-complete \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
