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

echo building core packages...
set ModuleName=core
set NetFxVersion=net40
::package ServiceBridge 
set NetCoreVersion=netstandard1.1
set PackageName=ServiceBridge
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.Interception
set NetCoreVersion=netstandard1.3
set PackageName=ServiceBridge.Interception
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.Activation
set NetCoreVersion=netstandard1.6
set PackageName=ServiceBridge.Activation
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.Interceptors
call :EnsureLibDir net40
call :EnsureLibDir net451
rmdir ..\src\core\ServiceBridge.Interceptors\bin /S /Q
call :BuildProject ..\src\core\ServiceBridge.Interceptors\ServiceBridge.Interceptors.net40.csproj
call :CopyLib ..\src\core\ServiceBridge.Interceptors\bin\%config% %cd%\lib\net40\ ServiceBridge.Interceptors
rmdir ..\src\core\ServiceBridge.Interceptors\bin /S /Q
call :BuildProject ..\src\core\ServiceBridge.Interceptors\ServiceBridge.Interceptors.net451.csproj
call :CopyLib ..\src\core\ServiceBridge.Interceptors\bin\%config% %cd%\lib\net451\ ServiceBridge.Interceptors
call :ExecuteCmd nuget.exe pack "%cd%\ServiceBridge.Interceptors.nuspec" -OutputDirectory %PackagesPath% %version%
call :ClearLibDir
IF %ERRORLEVEL% NEQ 0 goto error

echo building autofac implementation packages
set ModuleName=Autofac
set NetFxVersion=net45
::package ServiceBridge.Autofac
set NetCoreVersion=netstandard1.1
set PackageName=ServiceBridge.%ModuleName%
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.Autofac.Activation
set NetCoreVersion=netstandard1.6
set PackageName=ServiceBridge.%ModuleName%.Activation
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.Autofac.Interception
set PackageName=ServiceBridge.%ModuleName%.Interception
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

echo building Ninject implementation packages
set ModuleName=Ninject
set NetFxVersion=net40
::package ServiceBridge.Ninject
set NetCoreVersion=netstandard1.3
set PackageName=ServiceBridge.%ModuleName%
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.Ninject.Activation
set NetCoreVersion=netstandard1.6
set PackageName=ServiceBridge.%ModuleName%.Activation
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.Ninject.Interception
set PackageName=ServiceBridge.%ModuleName%.Interception
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

echo building StructureMap implementation packages
set ModuleName=StructureMap
set NetFxVersion=net45
::package ServiceBridge.StructureMap
set NetCoreVersion=netstandard1.3
set PackageName=ServiceBridge.%ModuleName%
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.StructureMap.Activation
set NetCoreVersion=netstandard1.6
set PackageName=ServiceBridge.%ModuleName%.Activation
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.StructureMap.Interception
set PackageName=ServiceBridge.%ModuleName%.Interception
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

echo building Unity implementation packages
set ModuleName=Unity
set NetFxVersion=net45
::package ServiceBridge.Unity
set PackageName=ServiceBridge.%ModuleName%
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.Unity.Activation
set PackageName=ServiceBridge.%ModuleName%.Activation
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.Unity.Interception
set PackageName=ServiceBridge.%ModuleName%.Interception
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

echo building Windsor implementation packages
set ModuleName=Windsor
set NetFxVersion=net45
::package ServiceBridge.Windsor
set PackageName=ServiceBridge.%ModuleName%
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.Windsor.Activation
set PackageName=ServiceBridge.%ModuleName%.Activation
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.Windsor.Interception
set PackageName=ServiceBridge.%ModuleName%.Interception
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

echo building Asp.Net MVC integration packages
set ModuleName=Mvc
set NetFxVersion=net45

::package ServiceBridge.Mvc
set PackageName=ServiceBridge.%ModuleName%
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.Mvc.Activation
set PackageName=ServiceBridge.%ModuleName%.Activation
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

echo building WCF integration packages
set ModuleName=WCF
set NetFxVersion=net40

::package ServiceBridge.ServiceModel
set PackageName=ServiceBridge.ServiceModel
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.ServiceModel.Activation
set PackageName=ServiceBridge.ServiceModel.Activation
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

echo building WebForms integration packages
set ModuleName=Web
set NetFxVersion=net45
::package ServiceBridge.Web
set PackageName=ServiceBridge.%ModuleName%
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

echo building Asp.Net WebApi integration packages
set ModuleName=WebApi
set NetFxVersion=net45
::package ServiceBridge.WebApi
set PackageName=ServiceBridge.%ModuleName%
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.WebApi
set PackageName=ServiceBridge.%ModuleName%.Activation
call :Package
IF %ERRORLEVEL% NEQ 0 goto error

echo building Asp.Net Core integration packages

::package ServiceBridge.AspNetCore
call :EnsureLibDir netstandard1.6
rmdir ..\src\AspNetCore\ServiceBridge.AspNetCore\bin /S /Q
call :BuildProject ..\src\AspNetCore\ServiceBridge.AspNetCore\ServiceBridge.AspNetCore.xproj
call :CopyLib ..\src\AspNetCore\ServiceBridge.AspNetCore\bin\%config%\netstandard1.6 %cd%\lib\netstandard1.6\ ServiceBridge.AspNetCore
call :ExecuteCmd nuget.exe pack "%cd%\ServiceBridge.AspNetCore.nuspec" -OutputDirectory %PackagesPath% %version%
call :ClearLibDir
IF %ERRORLEVEL% NEQ 0 goto error

::package ServiceBridge.AspNetCore.Activation
call :EnsureLibDir netstandard1.6
rmdir ..\src\AspNetCore\ServiceBridge.AspNetCore.Activation\bin /S /Q
call :BuildProject ..\src\AspNetCore\ServiceBridge.AspNetCore.Activation\ServiceBridge.AspNetCore.Activation.xproj
call :CopyLib ..\src\AspNetCore\ServiceBridge.AspNetCore.Activation\bin\%config%\netstandard1.6 %cd%\lib\netstandard1.6\ ServiceBridge.AspNetCore.Activation
call :ExecuteCmd nuget.exe pack "%cd%\ServiceBridge.AspNetCore.Activation.nuspec" -OutputDirectory %PackagesPath% %version%
call :ClearLibDir
IF %ERRORLEVEL% NEQ 0 goto error

goto end

:Package
setlocal
set _NetFxProj=..\src\%ModuleName%\%PackageName%\%PackageName%.csproj
set _NetCoreProj=..\netcore\%PackageName%\%PackageName%.netcore.xproj
if exist %_NetFxProj% (
call :EnsureLibDir %NetFxVersion%
rmdir ..\src\%ModuleName%\%PackageName%\bin /S /Q
call :BuildProject %_NetFxProj%
call :CopyLib ..\src\%ModuleName%\%PackageName%\bin\%config% %cd%\lib\%NetFxVersion%\ %PackageName%
)
if exist %_NetCoreProj% (
call :EnsureLibDir %NetCoreVersion%
rmdir ..\netcore\%PackageName%\bin /S /Q
call :BuildProject ..\netcore\%PackageName%\%PackageName%.netcore.xproj
call :CopyLib ..\netcore\%PackageName%\bin\%config%\%NetCoreVersion% %cd%\lib\%NetCoreVersion%\ %PackageName%
)
call :ExecuteCmd nuget.exe pack "%cd%\%PackageName%.nuspec" -OutputDirectory %PackagesPath% %version%
call :ClearLibDir
exit /b %ERRORLEVEL%


:SimplePackage
setlocal
set _PackagePath=%1
set _PackageName=%2
set _Framework=%3
call :EnsureLibDir %_Framework%
call :ExecuteCmd %msbuild% ..\src\%_PackagePath%\%_PackageName%.csproj /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

copy ..\src\%_PackagePath%\bin\%config%\%_PackageName%.dll %cd%\lib\%_Framework%\ /Y
copy ..\src\%_PackagePath%\bin\%config%\%_PackageName%.xml %cd%\lib\%_Framework%\ /Y

call :ExecuteCmd nuget.exe pack "%cd%\%_PackageName%.nuspec" -OutputDirectory %PackagesPath% %version%
call :ClearLibDir
exit /b %ERRORLEVEL%

:CopyLib
setlocal
set _SourcePath=%1
set _TargetPath=%2
set _PackageName=%3
set _JsonPath=%_SourcePath%\%_PackageName%.deps.json
copy %_SourcePath%\%_PackageName%.dll %_TargetPath% /Y
copy %_SourcePath%\%_PackageName%.xml %_TargetPath% /Y
if exist %_JsonPath% copy %_JsonPath% %_TargetPath% /Y
exit /b %ERRORLEVEL%

:BuildProject
setlocal
set _ProjectPath=%1
call :ExecuteCmd %msbuild% %_ProjectPath% /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

exit /b %ERRORLEVEL%

:EnsureLibDir
setlocal
set _Framework=%1
set libtmp=%cd%\lib

if not exist %libtmp% mkdir %libtmp%
if not exist %libtmp%\%_Framework% mkdir %libtmp%\%_Framework%

exit /b %ERRORLEVEL%

:ClearLibDir
setlocal
set libtmp=%cd%\lib
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