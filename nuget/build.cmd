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

set PackagesPath="%cd%\packages"
if not exist %PackagesPath% mkdir %PackagesPath%

rem echo Building solution...
rem call :ExecuteCmd %msbuild% "..\ServiceBridge.sln" /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
rem IF %ERRORLEVEL% NEQ 0 goto error

echo Packaging...
rem Core packages
call :Package core\ServiceBridge ServiceBridge net40
IF %ERRORLEVEL% NEQ 0 goto error
call :Package core\ServiceBridge.Activation ServiceBridge.Activation net40
IF %ERRORLEVEL% NEQ 0 goto error
call :Package core\ServiceBridge.Interception ServiceBridge.Interception net40
IF %ERRORLEVEL% NEQ 0 goto error

call :EnsureLibDir net40
call :ExecuteCmd %msbuild% ..\src\core\ServiceBridge.Interceptors\ServiceBridge.Interceptors.net40.csproj /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
copy ..\src\core\ServiceBridge.Interceptors\bin\%config%\ServiceBridge.Interceptors.dll %cd%\lib\net40\ /Y
copy ..\src\core\ServiceBridge.Interceptors\bin\%config%\ServiceBridge.Interceptors.xml %cd%\lib\net40\ /Y
rmdir ..\src\core\ServiceBridge.Interceptors\bin\%config% /S /Q
IF %ERRORLEVEL% NEQ 0 goto error
call :EnsureLibDir net451
call :ExecuteCmd %msbuild% ..\src\core\ServiceBridge.Interceptors\ServiceBridge.Interceptors.net451.csproj /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
copy ..\src\core\ServiceBridge.Interceptors\bin\%config%\ServiceBridge.Interceptors.dll %cd%\lib\net451\ /Y
copy ..\src\core\ServiceBridge.Interceptors\bin\%config%\ServiceBridge.Interceptors.xml %cd%\lib\net451\ /Y
rmdir ..\src\core\ServiceBridge.Interceptors\bin\%config% /S /Q
call :ExecuteCmd nuget.exe pack "%cd%\ServiceBridge.Interceptors.nuspec" -OutputDirectory %PackagesPath% %version%
IF %ERRORLEVEL% NEQ 0 goto error
rmdir %cd%\lib /S /Q

rem Integration packages
call :Package WCF\ServiceBridge.ServiceModel ServiceBridge.ServiceModel net40
IF %ERRORLEVEL% NEQ 0 goto error
call :Package WCF\ServiceBridge.ServiceModel.Activation ServiceBridge.ServiceModel.Activation net40
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Mvc\ServiceBridge.Mvc ServiceBridge.Mvc net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Mvc\ServiceBridge.Mvc.Activation ServiceBridge.Mvc.Activation net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Web\ServiceBridge.Web ServiceBridge.Web net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package WebApi\ServiceBridge.WebApi ServiceBridge.WebApi net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package WebApi\ServiceBridge.WebApi.Activation ServiceBridge.WebApi.Activation net45

rem Implementation packages
call :Package Autofac\ServiceBridge.Autofac ServiceBridge.Autofac net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Autofac\ServiceBridge.Autofac.Interception ServiceBridge.Autofac.Interception net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Autofac\ServiceBridge.Autofac.Activation ServiceBridge.Autofac.Activation net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Ninject\ServiceBridge.Ninject ServiceBridge.Ninject net40
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Ninject\ServiceBridge.Ninject.Interception ServiceBridge.Ninject.Interception net40
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Ninject\ServiceBridge.Ninject.Activation ServiceBridge.Ninject.Activation net40
IF %ERRORLEVEL% NEQ 0 goto error
call :Package StructureMap\ServiceBridge.StructureMap ServiceBridge.StructureMap net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package StructureMap\ServiceBridge.StructureMap.Interception ServiceBridge.StructureMap.Interception net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package StructureMap\ServiceBridge.StructureMap.Activation ServiceBridge.StructureMap.Activation net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Unity\ServiceBridge.Unity ServiceBridge.Unity net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Unity\ServiceBridge.Unity.Interception ServiceBridge.Unity.Interception net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Unity\ServiceBridge.Unity.Activation ServiceBridge.Unity.Activation net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Windsor\ServiceBridge.Windsor ServiceBridge.Windsor net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Windsor\ServiceBridge.Windsor.Interception ServiceBridge.Windsor.Interception net45
IF %ERRORLEVEL% NEQ 0 goto error
call :Package Windsor\ServiceBridge.Windsor.Activation ServiceBridge.Windsor.Activation net45
IF %ERRORLEVEL% NEQ 0 goto error

goto end

:Package
setlocal
set _PackagePath=%1
set _PackageName=%2
set _Framework=%3
set libtmp=%cd%\lib
call :EnsureLibDir %_Framework%
call :ExecuteCmd %msbuild% ..\src\%_PackagePath%\%_PackageName%.csproj /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

copy ..\src\%_PackagePath%\bin\%config%\%_PackageName%.dll %libtmp%\%_Framework%\ /Y
copy ..\src\%_PackagePath%\bin\%config%\%_PackageName%.xml %libtmp%\%_Framework%\ /Y

call :ExecuteCmd nuget.exe pack "%cd%\%_PackageName%.nuspec" -OutputDirectory %PackagesPath% %version%
rmdir %libtmp% /S /Q
exit /b %ERRORLEVEL%

:EnsureLibDir
setlocal
set _Framework=%1
set libtmp=%cd%\lib

if not exist %libtmp% mkdir %libtmp%
if not exist %libtmp%\%_Framework% mkdir %libtmp%\%_Framework%

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