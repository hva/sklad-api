$cmd = "$env:APPLICATION_PATH" + "\Sklad.Api.exe"

& $cmd "uninstall"
& $cmd "install"
& $cmd "start"
