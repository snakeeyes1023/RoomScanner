@echo off

set IP=%1

if "%IP%" == "" (
  echo Veuillez entrer l'adresse IP du Raspberry Pi en paramètre.
  set /p IP=Adresse IP du Raspberry Pi:
)

scp -r jcote@%IP%:/var/www .