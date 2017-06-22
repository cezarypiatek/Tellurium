FROM microsoft/windowsservercore
COPY InstallTellurium.ps1 c:/InstallTellurium.ps1
COPY RunTellurium.ps1 c:/RunTellurium.ps1
RUN powershell -ExecutionPolicy bypass -NoProfile -File c:/InstallTellurium.ps1
EXPOSE 5500
ENTRYPOINT powershell -ExecutionPolicy bypass -NoProfile -File c:/RunTellurium.ps1