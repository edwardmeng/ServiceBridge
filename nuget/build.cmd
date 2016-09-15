@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
) else (
   set version=-Version 1.0.0
)
REM Determine msbuild path
set msbuildtmp="%ProgramFiles%\MSBuild\14.0\bin\msbuild"
if exist %msbuildtmp% set msbuild=%msbuildtmp%
set msbuildtmp="%ProgramFiles(x86)%\MSBuild\14.0\bin\msbuild"
if exist %msbuildtmp% set msbuild=%msbuildtmp%
set VisualStudioVersion=14.0

REM Package restore
echo.
echo Running package restore...
call :ExecuteCmd nuget.exe restore ..\ServiceBridge.sln -OutputDirectory ..\packages -NonInteractive -ConfigFile nuget.config
IF %ERRORLEVEL% NEQ 0 goto error

echo Building solution...
call :ExecuteCmd %msbuild% "..\ServiceBridge.sln" /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
IF %ERRORLEVEL% NEQ 0 goto error

echo Packaging...
rem Core packages
call :Package core\ServiceBridge ServiceBridge net40
call :Package core\ServiceBridge.Interception ServiceBridge.Interception net40

rem Integration packages
call :Package WCF\ServiceBridge.ServiceModel ServiceBridge.ServiceModel net40
call :Package WCF\ServiceBridge.ServiceModel.Activation ServiceBridge.ServiceModel.Activation net40
call :Package Mvc\ServiceBridge.Mvc ServiceBridge.Mvc net45
call :Package Mvc\ServiceBridge.Mvc.Activation ServiceBridge.Mvc.Activation net45
call :Package Web\ServiceBridge.Web ServiceBridge.Web net45
call :Package WebApi\ServiceBridge.WebApi ServiceBridge.WebApi net45
call :Package WebApi\ServiceBridge.WebApi.Activation ServiceBridge.WebApi.Activation net45

rem Implementation packages
call :Package Autofac\ServiceBridge.Autofac ServiceBridge.Autofac net45
call :Package Autofac\ServiceBridge.Autofac.Interception ServiceBridge.Autofac.Interception net45
call :Package Autofac\ServiceBridge.Autofac.Activation ServiceBridge.Autofac.Activation net45
call :Package Ninject\ServiceBridge.Ninject ServiceBridge.Ninject net40
call :Package Ninject\ServiceBridge.Ninject.Interception ServiceBridge.Ninject.Interception net40
call :Package Ninject\ServiceBridge.Ninject.Activation ServiceBridge.Ninject.Activation net40
call :Package StructureMap\ServiceBridge.StructureMap ServiceBridge.StructureMap net45
call :Package StructureMap\ServiceBridge.StructureMap.Interception ServiceBridge.StructureMap.Interception net45
call :Package StructureMap\ServiceBridge.StructureMap.Activation ServiceBridge.StructureMap.Activation net45
call :Package Unity\ServiceBridge.Unity ServiceBridge.Unity net45
call :Package Unity\ServiceBridge.Unity.Interception ServiceBridge.Unity.Interception net45
call :Package Unity\ServiceBridge.Unity.Activation ServiceBridge.Unity.Activation net45
call :Package Windsor\ServiceBridge.Windsor ServiceBridge.Windsor net45
call :Package Windsor\ServiceBridge.Windsor.Interception ServiceBridge.Windsor.Interception net45
call :Package Windsor\ServiceBridge.Windsor.Activation ServiceBridge.Windsor.Activation net45

goto end

:Package
set local
set _PackagePath=%1
set _PackageName=%2
set _Framework=%3
set libtmp=%cd%\lib
set packagestmp="%cd%\packages"
set output=%cd%\..\output
if not exist %libtmp% mkdir %libtmp%
if not exist %packagestmp% mkdir %packagestmp%
if not exist %libtmp%\net461 mkdir %libtmp%\%_Framework%
if not exist %output%\%config% mkdir %output%\%config%

copy ..\src\%_PackagePath%\bin\%config%\%_PackageName%.dll %libtmp%\%_Framework%\ /Y
copy ..\src\%_PackagePath%\bin\%config%\%_PackageName%.xml %libtmp%\%_Framework%\ /Y
copy ..\src\%_PackagePath%\bin\%config%\%_PackageName%.dll %output%\%config%\%_PackageName%.dll /Y
copy ..\src\%_PackagePath%\bin\%config%\%_PackageName%.xml %output%\%config%\%_PackageName%.xml /Y

call :ExecuteCmd nuget.exe pack "%cd%\%_PackageName%.nuspec" -OutputDirectory %packagestmp% %version%
rmdir %libtmp% /S /Q
exit /b %ERRORLEVEL%

:: Execute command routine that will echo out when error
:ExecuteCmd
setlocal
set _CMD_=%*
call %_CMD_%
if "%ERRORLEVEL%" NEQ "0" echo Failed exitCode=%ERRORLEVEL%, command=%_CMD_%
exit /b %ERRORLEVEL%

:error
endlocal
echo An error has occurred during build.
call :exitSetErrorLevel
call :exitFromFunction 2>nul

:exitSetErrorLevel
exit /b 1

:exitFromFunction
()

:end
endlocal
echo Build finished successfully.