# What it this?
Umbraco 7.3 introduced [Flexible Load Balancing](https://our.umbraco.org/documentation/getting-started/setup/server-setup/load-balancing/flexible) which allowed two instances of the same Umbraco website to look at the same database with changes synced between the two website. However, media file uploads remain on the master website, they are not pushed to the load balanced site.
This is an extension to Umbraco to FTP media files up to the Azure Web App load balanced website on save in the master website.
#Implementation
One day I might make a package, for now follow these steps

1. Download and build this solution
2. Copy the 4 App settings from app.config to your Umbraco web.config and populate with your FTP credentials. (Ensure Enabled is false in the load balanced Web App)
3. Copy the Umbraco.Azure.MediaFtpSync.dll to the bin directory in the Umbraco website

