@echo off
..\CekaCli.exe -m=arff -i=uhs_evol3 -f=rebuildAttributeAsRanged -p=0 -p=120 -bw
..\CekaCli.exe -m=arff -i=uhs_evol3 -f=rebuildAttributeAsRanged -p=1 -p=120 -bw
..\CekaCli.exe -m=arff -i=uhs_evol3 -f=rebuildAttributeAsRanged -p=2 -p=16 -bw
pause