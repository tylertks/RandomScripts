$answer1 = Read-Host -Prompt "Enter your number"
$num = [int]$answer1
$answer2 = Read-Host -Prompt "Enter your factorial degree"
$factor = [int]$answer2

if($num -gt 0){
    if($factor -gt 1){
        $mod = $num % $factor
        $product = 1
        for ($i = 1; $i -lt $num+1; $i++) {
            $j = $i % $factor
            if($j -eq $mod){
                $product*=$i
            }
        }
        $f=""
        for ($i = 1; $i -lt $factor+1; $i++) {
            $f+="!"
        }
        Write-Host $num$f" = "$product
    }
    elseif ($factor -eq 1) {
        $product = 1
        for ($i = 1; $i -lt $num+1; $i++) {
            $product*=$i
        }
        Write-Host $num"! = "$product
    }
    else{
        Write-Host "Invalid Factor"
        break;
    }
}
else{
    Write-Host "Invalid Number"
    break;
}
