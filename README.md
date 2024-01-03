
# dotnet-efcore-mongodb

![enter image description here](/docs/swagger.png)

**Database** : BookStore

**Collection** : Books

## Endpoints

**GetAll**
- Response
	```json
	{
	  "results": [
	    {
	      "id": "6595cca8c14310e0a895b738",
	      "name": "El Principito",
	      "pages": 96,
	      "createdAt": "2024-01-03T21:07:52.9538058+00:00"
	    }
	  ],
	  "total": 1
	}
	```
**Get**
- Param
	```csharp
	string id
	```
- Response	
	```json
	{
		"id": "6595cca8c14310e0a895b738",
		"name": "El Principito",
		"pages": 96,
		"createdAt": "2024-01-03T21:07:52.9538058+00:00"
	}
	```
**Update**
- Param 
	```csharp
	string id
	```
- Request
	```json
	{
		 "name": "string",
		 "pages": 0
	}
	```   
 - Response
	```json
	{
		"id": "6595cca8c14310e0a895b738",
		"name": "El Principito Updated",
		"pages": 98,
		"createdAt": "2024-01-03T21:07:52.9538058+00:00"
	}
	```
**Delete**
-	Param 
	```csharp
	string id
	```  
 - Response
	```json
	{
		"id": "6595cca8c14310e0a895b738",
		"name": "El Principito Updated",
		"pages": 98,
		"createdAt": "2024-01-03T21:07:52.9538058+00:00"
	}
	```	