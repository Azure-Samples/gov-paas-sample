---
services: government
platforms: aspnetcore2.0
author: yujhong
---

# Azure Government PaaS Sample
This sample shows how to build an ASP.NET Core 2.0 MVC web application that uses Azure AD for sign-in using the OpenID Connect protocol, reads from and writes to an Azure SQL Database, writes to a Queue in Azure Storage, and uses a Redis Cache.

## How To Run This Sample
Getting started is simple!  To run this sample in Azure Government you will need:

- An Azure Active Directory (Azure AD) tenant in Azure Government. You must have an [Azure Government subscription](https://azure.microsoft.com/overview/clouds/government/request/) in order to have an AAD tenant in Azure Government. For more information on how to get an Azure AD tenant, please see [How to get an Azure AD tenant](https://azure.microsoft.com/en-us/documentation/articles/active-directory-howto-tenant/) 
- A user account in your Azure AD tenant. This sample will not work with a Microsoft account, so if you signed in to the Azure Government portal with a Microsoft account and have never created a user account in your directory before, you need to do that now.


To run locally you will additionally need:
- Install [.NET Core](https://www.microsoft.com/net/core) 2.0.0 or later.
- Install [Visual Studio](https://www.visualstudio.com/vs/) 2017 version 15.3 or later with the following workloads:
    - **ASP.NET and web development**
    - **.NET Core cross-platform development**

### Run and Test Sample in Azure Government 
#### Step 1: Deploy Resources to Azure Government

<a href="https://portal.azure.us/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fyujhongmicrosoft%2Fgov-paas-sample%2Fmaster%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/AzureGov.png" />
</a> 

#### Step 2:  Register the sample with your Azure Active Directory tenant

1. Sign in to the [Azure Government portal](https://portal.azure.us).
2. On the top bar, click on your account and under the **Directory** list, choose the Active Directory tenant where you wish to register your application.
3. Click on **More Services** in the left hand nav, and choose **Azure Active Directory**.
4. Click on **App registrations** and choose **Add**.
5. Enter a friendly name for the application, for example 'Inventory App' and select 'Web Application and/or Web API' as the Application Type. For the sign-on URL, enter the base URL for the sample, which is by default `http://localhost:57062/signin-oidc`. 

    >[!Note] 
    > We will change this URL later after creating the web application and deploying to Azure Government.
    >
    >

    Click on **Create** to create the application.
6. While still in the Azure portal, choose your application, click on **Settings** and choose **Properties**.
7. Find the Application ID value and copy it to the clipboard.
8. For the App ID URI, enter https://\<your_tenant_name\>/InventoryApp, replacing \<your_tenant_name\> with the name of your Azure AD tenant.
#### Step 3:  Configure the sample to use your Azure AD tenant
##### Azure Government Variations
The only variation when setting up AAD Authorization on the Azure Government cloud is in the AAD Instance:
 - "https://login.microsoftonline.us"

##### Configure the InventoryApp project
1. Login to the Azure Government portal and navigate to App Services -> Your Sample Web App -> Application Settings. 
2. Find the "App Settings" section. Add "Authentication:AzureAd:ClientId" as a key and your Web App's Client ID as the value. We can find the Client ID by navigating to AAD -> App Registrations -> Your Sample Web App -> Application ID.
3. Add "Authentication:AzureAd:TenantId" as a key and your Tenant ID as the value. We can find the Tenant ID by navigating to AAD -> Properties -> Directory Id. 
4. Add "Authentication:AzureAd:Domain" as a key and "<your AAD tenant name>.onmicrosoft.com" as the value. 
5. Add "ASPNETCORE_ENVIRONMENT" as a key and "Development" as the value.
6. Click "Save" on the top left corner of the page.

#### Step 4: Configure Azure SQL Database
##### Azure Government Variations
The only variation lies in the endpoint suffix when connecting to your Azure SQL Database:
- "database.usgovcloudapi.net"

We will create the table that the application will write to. In this project find the "ProductTable.sql" file and run the query on your Azure SQL Database (using a SQL Server tool such as SQL Server Management Studio). 

#### Step 5: Run Sample
Once you have gone through all of the steps above, you are ready to run your sample.

1. Navigate to the [Azure Government portal](https://portal.azure.us) and click on "Azure Active Directory" -> "App Registrations -> InventoryApp -> Reply URLS. Make sure the reply url is your application url with "/signin-oidc" at the end. You can get your application url by navigating to "App Services" from the portal. 
2. After the [sample has been deployed to your web app](https://docs.microsoft.com/en-us/azure/azure-government/documentation-government-howto-deploy-webandmobile#deploy-a-web-app-to-azure-government), you should be able to navigate to your app url and see that it ends in ".azurewebsites.us". 
3. After logging in with an account in your AAD tenant, you should see the InventoryApp main page. You should be able to create, edit, and delete items. 
4. If an item has the quantity of 0, the item will be written to your queue. You can see that this was done succesfully by using the [Azure Storage Explorer](https://docs.microsoft.com/azure/azure-government/documentation-government-get-started-connect-to-storage) or looking at your queue through the portal. 
5. The items with quantity 0 are also written to the redis cache, and when you click on the "Products to Restock" button the items will be read from the cache and displayed on the page.

### Run and Test Sample Locally

#### Step 1:  Clone or download this repository

From your shell or command line:

`git clone https://github.com/Azure-Samples/azure-gov-paas-sample.git

#### Step 2:  Register the sample with your Azure Active Directory tenant

The project in this sample needs to be registered in your Azure AD tenant.

##### Register the InventoryApp web application

1. Sign in to the [Azure Government portal](https://portal.azure.us).
2. On the top bar, click on your account and under the **Directory** list, choose the Active Directory tenant where you wish to register your application.
3. Click on **More Services** in the left hand nav, and choose **Azure Active Directory**.
4. Click on **App registrations** and choose **Add**.
5. Enter a friendly name for the application, for example 'Inventory App' and select 'Web Application and/or Web API' as the Application Type. For the sign-on URL, enter the base URL for the sample, which is by default `http://localhost:57062/signin-oidc`. 

    >[!Note] 
    > We will change this URL later after creating the web application and deploying to Azure Government.
    >
    >

    Click on **Create** to create the application.
6. While still in the Azure portal, choose your application, click on **Settings** and choose **Properties**.
7. Find the Application ID value and copy it to the clipboard.
8. For the App ID URI, enter https://\<your_tenant_name\>/InventoryApp, replacing \<your_tenant_name\> with the name of your Azure AD tenant.

#### Step 3:  Configure the sample to use your Azure AD tenant
##### Azure Government Variations
The only variation when setting up AAD Authorization on the Azure Government cloud is in the AAD Instance:
 - "https://login.microsoftonline.us"

##### Configure the InventoryApp project
1. Open the solution in Visual Studio 2017.
2. Open the `appsettings.json` file.
3. Find the `Authentication` section. We will be filling out the properties with your AAD tenant information.
4. Find the `ClientId` property and replace the value with the Client ID for the InventoryApp from the Azure Government portal. We can find the Client ID by navigating to AAD -> App Registrations -> InventoryApp -> Application ID. 
5. Find the `TenantId` property and replace the value with the Tenant ID for the InventoryApp from the Azure Government portal. We can find the Tenant ID by navigating to AAD -> Properties -> Directory Id. 
6. Find the `Domain` property and replace the value with "<tenantname>.onmicrosoft.com". 
6. Open the `startup.cs` file.
7. The services.AddAuthentication method is where the AAD authentication is added. 

#### Step 4: Connect to Azure SQL Database
##### Azure Government Variations
The only variation lies in the endpoint suffix when connecting to your Azure SQL Database:
- "database.usgovcloudapi.net"

>[!Note]
>If you clicked on the "Deploy to Azure Government" button, you can skip step 1.
>
>

1. Navigate to the [Azure Government Portal](https://portal.azure.us) and [create an Azure SQL Server and Database](https://docs.microsoft.com/en-us/azure/sql-database/). Make sure you save your server admin and password.
2. Now we must create the table that the application will write to. In this project find the "ProductTable.sql" file and run the query on your Azure SQL Database (using a SQL Server tool such as SQL Server Management Studio). 
3. Open the appsettings.json file and navigate to the `Connection Strings` section. 
4. Find the `DefaultConnection` property and replace the value with your Azure SQL Server connection string. In order to get the connection string, go to the Portal and navigate to your SQL Database -> connection strings. Grab the "primary" connection string and replace the User Id and Pasword properties with your server admin and password.	

#### Step 5: Connect to Azure Storage
##### Azure Government Variations
The only variation lies in the endpoint suffix when connecting to your Azure Government storage account. 
- "core.usgovcloudapi.net"

>[!Note]
>If you clicked on the "Deploy to Azure Government" button, you can skip step 1.
>
>

1. Navigate to the [Azure Government Portal](https://portal.azure.us) and [create an Azure Storage account](https://docs.microsoft.com/azure/storage/common/storage-create-storage-account). Once your storage account has been provisioned navigate to the Access Keys section on Storage Accounts and copy the access Key. Go back to your Storage Account in the portal and add a queue. 

2. Open up the `appsettings.json` file and navigate to the `Storage` section. Fill out the `AccountName` property with the name of your storage account. 
3. Fill out the `AccountKey` property with the name of the access Key for your storage account, which can be accessed through the [portal](https://portal.azure.us). 
4. Open up the `Startup.cs` file, and navigate to the `ConfigureServices` method. Here you can see that Azure storage was configured and connected to the application. 
5. Open up the `ProductsController.cs` file. Navigate to the `Restock` method. Replace the `<nameofQueue>` tag with the name of your queue. 

#### Step 6: Connect to Redis Cache 
##### Azure Government Variations
The only variation lies in the endpoint suffix when connecting to your Redis Cache in Azure Government.
- "redis.cache.usgovcloudapi.net" 

>[!Note]
>Even if you clicked on the "Deploy to Azure Government" button, **you must still provision a redis cache by following these steps.**
>
>

1. Navigate to the [Azure Government Portal](https://portal.azure.us), Click on the "New" button and type in "Redis Cache". Click "create" to provision your redis cache.
2. Click on your redis cache and navigate to the "Access Keys" section. Copy your Primary Connection String. 
3. Open up the `Appsettings.json` file. Navigate to the `ConnectionStrings` section and replace the value for `RedisConnection` with your primary connection string. 
4. Open up the `ProductsController.cs` file and navigate to the `Restock` and `RestockList` methods. The `Restock` method is writing the names of the items that need restocking to the cache, and the `RestockList` method is retrieving the stored names from the cache and displaying them.

#### Step 7:  Run the sample
Once you have gone through all of the steps above, you are ready to run your sample. 

1. Navigate to the [Azure Government portal](https://portal.azure.us) and click on "Azure Active Directory" -> "App Registrations -> InventoryApp -> Reply URLS. Make sure the reply url is "http://localhost:57062/signin-oidc". 
2. Run the sample on Visual Studio, and you should see the Microsoft login page appear. Make sure to login with the credentials for the tenant that you registered the app with.
3. After logging in, you should see the InventoryApp main page. You should be able to create, edit, and delete items. 
4. If an item has the quantity of 0, the item will be written to your queue. You can see that this was done succesfully by using the [Azure Storage Explorer](https://docs.microsoft.com/azure/azure-government/documentation-government-get-started-connect-to-storage) or looking at your queue through the portal. 
5. The items with quantity 0 are also written to the redis cache, and when you click on the "Products to Restock" button the items will be read from the cache and displayed on the page.


