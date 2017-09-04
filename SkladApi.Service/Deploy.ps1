$cmd = "$env:APPLICATION_PATH" + "\SkladApi.Service.exe"

& $cmd "uninstall"
& $cmd "install"
& $cmd "start"
