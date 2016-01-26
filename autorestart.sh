
#Restart Script
go='tmux send-keys -t mineserver'
today=$(date +"%m-%d-%Y")
timeleft=10
worldLoc="~/Stuff/ScrocheCraft/world"
backupLoc="~/Stuff/ScrocheCraft/world/world-backups/world"
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
done
$go "say RESTARTING" C-m
sleep 1s
$go "stop" C-m
sleep 30s
$go "cp -R ~/Stuff/ScrocheCraft/world/ ~/Stuff/ScrocheCraft/world/world-backups/world" C-m
$go "mv ~/Stuff/ScrocheCraft/world/world-backups/world ~Stuff/ScrocheCraft/world-backups/world_$today" C-m
$go "cd ~/Stuff/ScrocheCraft/ && ./launch.sh" C-m