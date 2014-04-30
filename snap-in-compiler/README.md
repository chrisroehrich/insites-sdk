Please see the Snap-In Framework documentation on http://doc.insitescloud.com/4.4/devdoc

snapin-compiler.jar requires at least JDK 1.7

snapin.keystore and snapin-compiler.jar must be kept in the same directory.

To compile and deploy a snap-in, use the snapin-compiler.jar, or the .bat or .sh convenience scripts:
java -jar snapin-compiler.jar -src ..\code-examples\snap-ins\src\insites-example -out ..\code-examples\snap-ins\out\insites-example.zip -deploy https://provider.dev.insitescloud.com -deployUser "admin" -deployPassword "password" -deployPort 443