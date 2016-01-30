#Restart Script
go='tmux send-keys -t mineserver'
today=$(date +"%Y %m %d %T")
timeleft=10
worldLoc="~/Stuff/ScrocheCraft/world/"
backupLoc="~/Stuff/ScrocheCraft/world_backups/"

#Server Restart Countdown
$go "say SERVER IS RESTARTING IN 5 MINUTES" C-m
sleep 180s
$go "say SERVER IS RESTARTING IN 2 MINUTES" C-m
sleep 60s
$go "say SERVER IS RESTARTING IN 1 MINUTE" C-m
sleep 30s
$go "say SERVER IS RESTARTING IN 30 SECONDS" C-m
sleep 10s
$go "say SERVER IS RESTARTING IN 20 SECONDS" C-m
sleep 10s
while (( $timeleft >= 1 ))
do
	$go "say $timeleft SECONDS" C-m
	timeleft=$(($timeleft - 1))
	sleep 1s
done
$go "say RESTARTING..." C-m
sleep 1s

#Stop the server
$go "stop" C-m

#wait to ensure the server is completely shutdown
sleep 30s

#Copy the world into the backup directory
cp -rv $worldLoc $backupLoc

#CD into the backup directory, and change the name of the backup to include the date
cd $backupLoc && mv world "world $today"

#CD into the server directory and relaunch the server
$go "cd ~/Stuff/ScrocheCraft/ && ./launch.sh" C-m
