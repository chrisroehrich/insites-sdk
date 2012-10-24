Please see the Snap-In Framework documentation on http://doc.intelligentinsites.net/devdoc

snapin.keystore and snapin-compiler.jar must be kept in the same directory.

To compile and deploy a snap-in, use the snapin-compiler.jar, or the .bat or .sh convenience scripts:
snapin-creator.bat -deploy <host> -deployPassword <pass> -deployPort 80 -deployUser <user> -src ..\code-examples\snap-ins\src\insites-example -out ..\code-examples\snap-ins\out\insites-example.zip