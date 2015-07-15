@echo off
..\CekaCli.exe -m=mine -v -a=apriori -i=uhs_evol3 -ot=json-file-pretty -o=uhs-res-json -p=confidence:0,5 -p=support:0,5 -p=apply-confidence:true -p=apply-support:true
pause