@echo off
..\CekaCli.exe -m=arff -i=uhs_evol3 -f=removePatternMatchRows -p="[-19<->101]"
..\CekaCli.exe -m=arff -i=uhs_evol3 -f=removePatternMatchRows -p="*" -p="[-67<->53]"
pause