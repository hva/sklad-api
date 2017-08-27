$cmd = "$env:APPLICATION_PATH" + "\SkladApi.Service.exe"

& $cmd "stop"
& $cmd "uninstall"
& $cmd "install"
& $cmd "start"
