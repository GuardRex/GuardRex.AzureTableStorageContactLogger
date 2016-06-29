# Azure Table Storage Contact Logger
Log contacts to Azure Table Storage.

The library provides a temporary approach to logging contacts to Azure Table Storage. This library should be replaced by `Serilog.Sinks.AzureTableStorage` when updated for `netstandard1.x/netcoreapp1.0`.

### Operation
To log contacts, use:
```c#
GuardRex.AzureTableStorageExceptionLogger.ExceptionLog.Log(
    string storageAccountName, 
    string storageAccountKey, 
    string ContactTableName, 
    string name,
    string email,
    string phone,
    string department,
    string message, 
    string result, 
    Serilog.ILogger logger = null)
```
If the table is not present it will be created automatically in the storage account provided. If the optional `Serilog.ILogger` is provided and there is an exception, the library will write the exception to the logger provided.

### Contact Log Table Schema

| Property     | Type     | Value                                      |
|--------------|----------|--------------------------------------------|
| PartitionKey | string   | Value "CT"                                 |
| RowKey*      | string   | DateTime.MaxValue - DateTime.UtcNow        |
| DT           | DateTime | DateTime.UtcNow                            |
| MachineName  | string   | Environment.MachineName                    |
| Application  | string   | Assembly.GetEntryAssembly().GetName().Name |
| Name         | string   | Provided                                   |
| Email        | string   | Provided                                   |
| Phone        | string   | Provided                                   |
| Department   | string   | Provided                                   |
| Message      | string   | Provided                                   |
| Result       | string   | Provided                                   |

*The calculated RowKey sorts the table from oldest to newest, which makes it easier to grab the top X number of entities when querying the table.

### Version History
Version | Changes Made
------- | ------------
1.0.0   | Initial Release
