@echo off
..\CekaCli.exe -m=mine -a=apriori -i=uhs_evol3 -ot=json -p=confidence:0,7 -p=support:0,7 -p=apply-confidence:true -p=apply-support:true -bw
pause