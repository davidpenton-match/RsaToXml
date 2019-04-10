@echo off
setlocal EnableDelayedExpansion

call publish.bat linux-x64
call publish.bat win-x64
call publish.bat osx-x64
