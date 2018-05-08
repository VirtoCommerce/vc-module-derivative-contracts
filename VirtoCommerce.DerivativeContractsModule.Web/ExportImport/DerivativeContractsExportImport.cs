using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.DerivativeContractsModule.Core.Services;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.DerivativeContractsModule.Web.ExportImport
{
    public class DerivativeContractsExportImport
    {
        private const int BatchSize = 50;
        private readonly JsonSerializer _serializer;

        private readonly IDerivativeContractService _derivativeContractService;
        private readonly IDerivativeContractSearchService _derivativeContractSearchService;

        public DerivativeContractsExportImport(IDerivativeContractService derivativeContractService, IDerivativeContractSearchService derivativeContractSearchService)
        {
            _derivativeContractService = derivativeContractService;
            _derivativeContractSearchService = derivativeContractSearchService;

            _serializer = new JsonSerializer();
            _serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            _serializer.Formatting = Formatting.Indented;
            _serializer.NullValueHandling = NullValueHandling.Ignore;
        }

        public void DoExport(Stream outStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
            progressCallback(progressInfo);

            using (StreamWriter sw = new StreamWriter(outStream, Encoding.UTF8))
            {
                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    writer.WriteStartObject();

                    ExportDerivativeContractEntity(writer, _serializer, manifest, progressInfo, progressCallback);
                    ExportDerivativeContractItemEntity(writer, _serializer, manifest, progressInfo, progressCallback);

                    writer.WriteEndObject();
                    writer.Flush();
                }
            }
        }

        private void ExportDerivativeContractEntity(JsonTextWriter writer, JsonSerializer serializer, PlatformExportManifest manifest, ExportImportProgressInfo progressInfo, Action<ExportImportProgressInfo> progressCallback)
        {
            progressInfo.Description = "Exporting DerivativeContracts...";
            progressCallback(progressInfo);

            var totalCount = _derivativeContractSearchService.SearchDerivativeContracts(new DerivativeContractSearchCriteria { Take = 0, Skip = 0 }).TotalCount;
            writer.WritePropertyName("DerivativeContractsTotalCount");
            writer.WriteValue(totalCount);

            writer.WritePropertyName("DerivativeContracts");
            writer.WriteStartArray();

            for (int i = 0; i < totalCount; i += BatchSize)
            {
                var results = _derivativeContractSearchService.SearchDerivativeContracts(new DerivativeContractSearchCriteria { Take = BatchSize, Skip = i }).Results;

                foreach (var result in results)
                {
                    serializer.Serialize(writer, result);
                }

                writer.Flush();
                progressInfo.Description = $"{Math.Min(totalCount, i + BatchSize)} of {totalCount} DerivativeContracts exported";
                progressCallback(progressInfo);
            }

            writer.WriteEndArray();
        }

        private void ExportDerivativeContractItemEntity(JsonTextWriter writer, JsonSerializer serializer, PlatformExportManifest manifest, ExportImportProgressInfo progressInfo, Action<ExportImportProgressInfo> progressCallback)
        {
            progressInfo.Description = "Exporting DerivativeContractItems...";
            progressCallback(progressInfo);

            var totalCount = _derivativeContractSearchService.SearchDerivativeContractItems(new DerivativeContractItemSearchCriteria { Take = 0, Skip = 0 }).TotalCount;
            writer.WritePropertyName("DerivativeContractItemsTotalCount");
            writer.WriteValue(totalCount);

            writer.WritePropertyName("DerivativeContractItems");
            writer.WriteStartArray();

            for (int i = 0; i < totalCount; i += BatchSize)
            {
                var results = _derivativeContractSearchService.SearchDerivativeContractItems(new DerivativeContractItemSearchCriteria { Take = BatchSize, Skip = i }).Results;

                foreach (var result in results)
                {
                    serializer.Serialize(writer, result);
                }

                writer.Flush();
                progressInfo.Description = $"{Math.Min(totalCount, i + BatchSize)} of {totalCount} DerivativeContractItems exported";
                progressCallback(progressInfo);
            }

            writer.WriteEndArray();
        }


        public void DoImport(Stream inputStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var progressInfo = new ExportImportProgressInfo();

            var contractsTotalCount = 0;
            var itemsTotalCount = 0;

            using (StreamReader streamReader = new StreamReader(inputStream))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                {
                    while (jsonReader.Read())
                    {
                        if (jsonReader.TokenType != JsonToken.PropertyName)
                            continue;

                        if (jsonReader.Value.ToString() == "DerivativeContractsTotalCount")
                        {
                            contractsTotalCount = jsonReader.ReadAsInt32() ?? 0;
                        }
                        else if (jsonReader.Value.ToString() == "DerivativeContracts")
                        {
                            jsonReader.Read();

                            //read contracts into internal list and batch save them save
                            var totalImportedContracts = 0;
                            if (jsonReader.TokenType == JsonToken.StartArray)
                            {
                                jsonReader.Read();

                                var derivativeContract = new List<DerivativeContract>();
                                while (jsonReader.TokenType != JsonToken.EndArray)
                                {
                                    var contract = _serializer.Deserialize<DerivativeContract>(jsonReader);
                                    derivativeContract.Add(contract);
                                    totalImportedContracts++;

                                    jsonReader.Read();

                                    if (derivativeContract.Count % BatchSize == 0 || jsonReader.TokenType == JsonToken.EndArray)
                                    {
                                        //save batch
                                        _derivativeContractService.SaveDerivativeContracts(derivativeContract.ToArray());
                                        derivativeContract.Clear();

                                        progressInfo.Description =  $"{ totalImportedContracts } of { contractsTotalCount } contracts imported";
                                        progressCallback(progressInfo);
                                    }
                                }
                            }
                         }
                        else if (jsonReader.Value.ToString() == "DerivativeContractItemsTotalCount")
                        {
                            itemsTotalCount = jsonReader.ReadAsInt32() ?? 0;
                        }
                        else if (jsonReader.Value.ToString() == "DerivativeContractItems")
                        {
                            jsonReader.Read();

                            //read items into internal list and batch save them save
                            var totalImportedItems = 0;
                            if (jsonReader.TokenType == JsonToken.StartArray)
                            {
                                jsonReader.Read();

                                var derivativeContractItems = new List<DerivativeContractItem>();
                                while (jsonReader.TokenType != JsonToken.EndArray)
                                {
                                    var item = _serializer.Deserialize<DerivativeContractItem>(jsonReader);
                                    derivativeContractItems.Add(item);
                                    totalImportedItems++;

                                    jsonReader.Read();

                                    if (derivativeContractItems.Count % BatchSize == 0 || jsonReader.TokenType == JsonToken.EndArray)
                                    {
                                        //save batch
                                        _derivativeContractService.SaveDerivativeContractItems(derivativeContractItems.ToArray());
                                        derivativeContractItems.Clear();

                                        progressInfo.Description = $"{ totalImportedItems } of { itemsTotalCount } items imported";
                                        progressCallback(progressInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}