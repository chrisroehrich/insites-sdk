StaffLister implements a Java Servlet which makes calls to the InSites Web Services API, and parses the results.
HTTP calls are made using the BasicClient class included in this package. You'll need to point 
this application at a running instance of the InSites Platform, which is a hard-coded URL. A real
implementation would have this value defined somewhere else.

It retrieves a list of all locations which are 'floors', then allows a user to select among them.
The servlet then gets all staff objects on the selected floor and displays them in a list, 5 at a time.

Required platform version: 4.1.0