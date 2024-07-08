pushd .
cd ../samples
echo ~~~~~~~~~~~~~~~PULLING SAMPLES~~~~~~~~~~~~~~~~
git pull
cd ../unity-hawk
echo ~~~~~~~~~~~~~PULLING UNITY-HAWK~~~~~~~~~~~~~~~
git pull
cd ../tools
echo ~~~~~~~~~~~~~~~~~~PULLING LIB~~~~~~~~~~~~~~~~~
git pull
popd
echo ~~~~~~~~~~~~~~~~~~PULLING MUT~~~~~~~~~~~~~~~~~
git pull
