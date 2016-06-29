using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using Serilog;

namespace GuardRex.AzureTableStorageContactLogger
{
    public class ContactLog : TableEntity
    {
        public ContactLog()
        {
        }

        public ContactLog(string ts)
        {
            PartitionKey = "CT";
            RowKey = ts;
        }

        public DateTime DT { get; set; }
        public string MachineName { get; set; }
        public string Application { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string Message { get; set; }
        public string Result { get; set; }

        public async static Task<bool> Log(
            string storageAccountName, 
            string storageAccountKey, 
            string contactTableName, 
            string name,
            string email,
            string phone,
            string department,
            string message, 
            string result,
            ILogger logger = null)
        {
            DateTime dt = DateTime.UtcNow;
            try
            {
                StorageCredentials storageCredentials = new StorageCredentials(storageAccountName, storageAccountKey);
                CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                tableClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(1), 10);
                CloudTable table_Contact = tableClient.GetTableReference(contactTableName);
                await table_Contact.CreateIfNotExistsAsync();
                ContactLog entity = new ContactLog((DateTime.MaxValue - dt).ToString());
                entity.DT = dt;
                entity.MachineName = Environment.MachineName;
                entity.Application = Assembly.GetEntryAssembly().GetName().Name;
                entity.Name = name;
                entity.Email = email;
                entity.Phone = phone;
                entity.Department = department;
                entity.Message = message;
                entity.Result = result;
                TableResult operationResult = await table_Contact.ExecuteAsync(TableOperation.Insert(entity));
                if (operationResult.HttpStatusCode != 201 && logger != null)
                {
                    logger.Error(operationResult.HttpStatusCode.ToString() + " " + operationResult.Result.ToString());
                }
                return operationResult.HttpStatusCode == 201 ? true : false;
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.Error(ex.ToString());
                }
                Console.WriteLine(dt.ToString() + " " + Environment.MachineName + ":" + Assembly.GetEntryAssembly().GetName().Name + ": " + ex.ToString());
                return false;
            }
        }
    }
}
