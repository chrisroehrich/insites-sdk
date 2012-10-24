RTLSDevice is an Android application which makes calls to the InSites Web Services API. The purpose 
is to demonstrate a third-party system relaying sensor information to the InSties Platform.

The application allows you to specify connection credentials, and the label of an existing sensor 
tracked by InSites, then do any of the following:

-simulate a sensor button press
-simulate a sensor changing locations
-send a low battery indication


To run it from Eclipse (Juno):
- Install the Android Development Tools plugin for Eclipse.
- Install the Android SDK version 4.1
- Window -> Preferences -> Android. Set the SDK Location to your Android SDK folder and hit Apply.
- In the Project Explorer, right click the RTLSDevice project -> Android Tools -> Fix Project Properties.
- Add a new Android Virtual Device using the Android Virtual Device Manager. Eclipse will remind you of this when attempting to run the application.
- (optional) The sensor provider name is hard-coded in the API calls as "other-rtls". You may want to change it.
- Run -> Run As -> Android Application

