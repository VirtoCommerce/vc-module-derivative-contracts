using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.DerivativeContractsModule.Core.Services;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.DerivativeContractsModule.Web.ExportImport
{
    public class DerivativeContractsExportImport
    {
        private const int _batchSize = 20;
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

            for (int i = 0; i < totalCount; i += _batchSize)
            {
                var results = _derivativeContractSearchService.SearchDerivativeContracts(new DerivativeContractSearchCriteria { Take = _batchSize, Skip = i }).Results;

                foreach (var result in results)
                {
                    serializer.Serialize(writer, result);
                }

                writer.Flush();
                progressInfo.Description = $"{Math.Min(totalCount, i + _batchSize)} of {totalCount} DerivativeContracts exported";
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

            for (int i = 0; i < totalCount; i += _batchSize)
            {
                var results = _derivativeContractSearchService.SearchDerivativeContractItems(new DerivativeContractItemSearchCriteria { Take = _batchSize, Skip = i }).Results;

                foreach (var result in results)
                {
                    serializer.Serialize(writer, result);
                }

                writer.Flush();
                progressInfo.Description = $"{Math.Min(totalCount, i + _batchSize)} of {totalCount} DerivativeContractItems exported";
                progressCallback(progressInfo);
            }

            writer.WriteEndArray();
        }

        public void DoImport(Stream inputStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback)
        {
            var progressInfo = new ExportImportProgressInfo();

            using (StreamReader streamReader = new StreamReader(inputStream))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                {
                    while (jsonReader.Read())
                    {
                        if (jsonReader.TokenType != JsonToken.PropertyName)
                            continue;

                        if (jsonReader.Value.ToString() == "DerivativeContracts")
                        {
                            jsonReader.Read();
                            var derivativeContracts = _serializer.Deserialize<DerivativeContract[]>(jsonReader);
                            progressInfo.Description = $"Importing {derivativeContracts.Length} DerivativeContracts...";
                            progressCallback(progressInfo);
                            _derivativeContractService.SaveDerivativeContracts(derivativeContracts);
                        } else if (jsonReader.Value.ToString() == "DerivativeContractItems")
                        {
                            jsonReader.Read();
                            var derivativeContractItems = _serializer.Deserialize<DerivativeContractItem[]>(jsonReader);
                            progressInfo.Description = $"Importing {derivativeContractItems.Length} DerivativeContractItem...";
                            progressCallback(progressInfo);
                            _derivativeContractService.SaveDerivativeContractItems(derivativeContractItems);
                        }
                    }
                }
            }
        }
    }
}