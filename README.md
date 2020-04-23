# lotus

Lotus is a work in progress concept.  We'll evolve it as we put more thought into it.

If you want to run it - then you'll need to create an Azure Cosmos DB Database and put the details into settings.json where I have left placeholders.  

You'll also need to add the key for the CosmosDb in wither a local.settings.json file (copy always and exclude this from source control) or you can set an environment variable to contain the key called CosmosContext__AuthKey. All these settingss will be merged together at runtime formt he set needed for execution. 