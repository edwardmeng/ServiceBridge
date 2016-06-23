@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
) else (
   set version=-Version 1.0.0-beta
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
call :ExecuteCmd ..\tools\nuget.exe restore ..\Wheatech.ServiceModel.sln -OutputDirectory ..\packages -NonInteractive -ConfigFile nuget.config
IF %ERRORLEVEL% NEQ 0 goto error

echo Building solution...
call :ExecuteCmd %msbuild% "..\Wheatech.ServiceModel.sln" /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
IF %ERRORLEVEL% NEQ 0 goto error

echo Packaging...
rem Core packages
call :Package Wheatech.ServiceModel
call :Package Wheatech.ServiceModel.Interception
call :Package Wheatech.ServiceModel.Wcf
call :Package Wheatech.ServiceModel.Mvc

rem Integration packages
call :Package Wheatech.ServiceModel.Autofac
call :Package Wheatech.ServiceModel.Autofac.Interception
call :Package Wheatech.ServiceModel.Ninject
call :Package Wheatech.ServiceModel.Ninject.Interception
call :Package Wheatech.ServiceModel.StructureMap
call :Package Wheatech.ServiceModel.StructureMap.Interception
call :Package Wheatech.ServiceModel.Unity
call :Package Wheatech.ServiceModel.Unity.Interception
call :Package Wheatech.ServiceModel.Windsor
call :Package Wheatech.ServiceModel.Windsor.Interception

goto end

:Package
set local
set _PackageName=%1
set libtmp=%cd%\lib
set packagestmp="%cd%\packages"
if not exist %libtmp% mkdir %libtmp%
if not exist %packagestmp% mkdir %packagestmp%
if not exist %libtmp%\net461 mkdir %libtmp%\net461

copy ..\src\%_PackageName%\bin\%config%\%_PackageName%.dll %libtmp%\net461 /Y
copy ..\src\%_PackageName%\bin\%config%\%_PackageName%.xml %libtmp%\net461 /Y
call :ExecuteCmd ..\tools\nuget.exe pack "%cd%\%_PackageName%.nuspec" -OutputDirectory %packagestmp% %version%
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