@echo off
setlocal EnableDelayedExpansion

if [%1]==[] goto BLANK

dotnet publish RsaToXml/RsaToXml.csproj -c Release --self-contained -r %1

GOTO DONE

:BLANK

ECHO No Parameter

GOTO DONE

:DONE
