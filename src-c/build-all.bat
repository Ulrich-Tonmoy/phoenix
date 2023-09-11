@ECHO OFF
REM Build Everything

ECHO "Building All Projects..."


PUSHD engine
CALL build.bat
POPD
IF %ERRORLEVEL% NEQ 0 (echo Error:%ERRORLEVEL% && exit)

PUSHD test
CALL build.bat
POPD
IF %ERRORLEVEL% NEQ 0 (echo Error:%ERRORLEVEL% && exit)

ECHO "All Assemblies Built Successfully."