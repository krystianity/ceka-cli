@echo off
..\CekaCli.exe -m=mine -v -l -a=apriori -i=uhs_evol3 -ot=weka-file -o=uhs-res-weka -p=confidence:0,5 -p=support:0,5 -p=apply-confidence:true -p=apply-support:true
pause