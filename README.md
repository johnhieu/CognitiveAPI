# CognitiveAPI
Web API for Cognitive Reposity using OData as a way to send data as JSON.

This API uses EF 6 since OData requires it in order to generate Controller for each model class Model Folders . By using the connection string, it will use the database and generate Model in the Model Folder (Using Database First Approach). Therefore, most of functionalities are in the controller classes. For custom functions, I place those in the accountsController class (you can put it anywhere as you want) and config them in the WebAPIConfig class to allow these functions to be called from other domain.

Also, to let other site to call our web API, I use Cross Origin Resource Sharing (CORS) which is a W3C standard that allows a server to relax the same-origin policy. You can check out the comments in the controller to see it.



