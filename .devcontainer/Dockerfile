# See here for image contents: https://github.com/microsoft/vscode-dev-containers/tree/v0.238.1/containers/dotnet/.devcontainer/base.Dockerfile

# [Choice] .NET version: 6.0, 5.0, 3.1, 6.0-bullseye, 5.0-bullseye, 3.1-bullseye, 6.0-focal, 5.0-focal, 3.1-focal
ARG VARIANT=6.0-bullseye
FROM mcr.microsoft.com/vscode/devcontainers/dotnet:0-${VARIANT}

# Install missing .NET SDK
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 7.0 --quality preview --install-dir /usr/share/dotnet

# Install mono
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends mono-complete \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
