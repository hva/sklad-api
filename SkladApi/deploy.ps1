$cmd = "$env:APPLICATION_PATH" + "\SkladApi.exe"

& $cmd "uninstall"
& $cmd "install"
& $cmd "start"
