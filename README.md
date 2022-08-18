# Phase One Records

![PaperStreetSoap banner image](https://paperstreetsoap.blob.core.windows.net/site/phaseonerecords.png)

https://paperstreetsoap.azurewebsites.net/

A .NET 6 MVC Web App for a client promoting and selling their online trading resources.
The client requested a platform where the user could register, select a package and purchase the package using Bitcoin.
Upon successful payment, the user would have access to members only content, including embedded videos, live trading data and downloadable resources.

Features of the Web App includes:

* A custom Content Management System
* User login, registration and subscription payment, handled consuming a Bitcoin Node Rest API client
* An Azure SQL database
* An Azure CDN with Storage Containers and Blobs to serve all JS, CSS and Images
* Notification emails sent out to users whenever new content is uploaded using a Google SMTP Client

Tech Stack inludes:

* .NET 6
* C#
* MVC
* Azure SQL
* Azure CDN
* Dapper
* RestShap API
* Rollbar Error Handling
* jQuery
* Bootstrap 5
