
#Restart Script
go='tmux send-keys -t mineserver'
today=$(date +"%m-%d-%Y")
timeleft=10
worldLoc="~/Stuff/ScrocheCraft/world"
backupLoc="~/Stuff/ScrocheCraft/world-backups/world"
$go "say SERVER IS RESTARTING IN 5 MINUTES" C-m
sleep 3m
$go "say SERVER IS RESTARTING IN 2 MINUTES" C-m
sleep 1m
$go "say SERVER IS RESTARTING IN 1 MINUTE" C-m
sleep 30s
$go "say SERVER IS RESTARTING IN 30 SECONDS" C-m
sleep 10s
$go "say SERVER IS RESTARTING IN 20 SECONDS" C-m
sleep 10s
while [$ -gt 0]
do
	$go "say $timeleft SECONDS" C-m
	a=`expr $timeleft - 1`
done
$go "say RESTARTING" C-m
sleep 1s
$go "stop" C-m
sleep 30s
$go "cp -rv $worldLoc $backupLoc" C-m
$go "cd $backupLoc && mv world world_$today" C-m
$go "cd ~/Stuff/ScrocheCraft/ && ./launch.sh" C-m
