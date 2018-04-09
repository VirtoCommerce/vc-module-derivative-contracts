﻿using System;
using System.Linq;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.DerivativeContractsModule.Core.Services;
using VirtoCommerce.DerivativeContractsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.DerivativeContractsModule.Data.Services
{
    public class DerivativeContractInfoEvaluator : IDerivativeContractInfoEvaluator
    {
        private readonly Func<IDerivativeContractRepository> _repositoryFactory;

        public DerivativeContractInfoEvaluator(Func<IDerivativeContractRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        #region Implementation of IDerivativeContractInfoEvaluator

        public virtual DerivativeContractInfo[] EvaluateDerivativeInfos(DerivativeContractInfoEvaluationContext evaluationContext)
        {
            using (var repository = _repositoryFactory())
            {
                repository.DisableChangesTracking();

                var query = repository.DerivativeContractItems;

                if (!string.IsNullOrEmpty(evaluationContext.MemberId))
                {
                    query = query.Where(dci => evaluationContext.MemberId == dci.DerivativeContract.MemberId);
                }

                if (!evaluationContext.ProductIds.IsNullOrEmpty())
                {
                    query = query.Where(dci => evaluationContext.ProductIds.Contains(dci.ProductId));
                }

                if (!evaluationContext.Types.IsNullOrEmpty())
                {
                    var evaluationContextTypes = evaluationContext.Types.Select(x => x.ToString()).ToArray();
                    query = query.Where(dci => evaluationContextTypes.Contains(dci.DerivativeContract.Type));
                }

                if (evaluationContext.StartDateRange?.EarlierDate != null)
                {
                    query = query.Where(dci => evaluationContext.StartDateRange.IncludeEarlier ? evaluationContext.StartDateRange.EarlierDate <= dci.DerivativeContract.StartDate : evaluationContext.StartDateRange.EarlierDate < dci.DerivativeContract.StartDate);
                }

                if (evaluationContext.StartDateRange?.LaterDate != null)
                {
                    query = query.Where(dci => evaluationContext.StartDateRange.IncludeLater ? dci.DerivativeContract.StartDate >= evaluationContext.StartDateRange.LaterDate : dci.DerivativeContract.StartDate > evaluationContext.StartDateRange.LaterDate);
                }

                if (evaluationContext.EndDateRange?.EarlierDate != null)
                {
                    query = query.Where(dci => evaluationContext.EndDateRange.IncludeEarlier ? evaluationContext.EndDateRange.EarlierDate <= dci.DerivativeContract.EndDate : evaluationContext.EndDateRange.EarlierDate < dci.DerivativeContract.EndDate);
                }

                if (evaluationContext.EndDateRange?.LaterDate != null)
                {
                    query = query.Where(dci => evaluationContext.EndDateRange.IncludeLater ? dci.DerivativeContract.EndDate >= evaluationContext.EndDateRange.LaterDate : dci.DerivativeContract.EndDate > evaluationContext.EndDateRange.LaterDate);
                }

                if (evaluationContext.OnlyActive)
                {
                    var now = DateTime.Now;
                    query = query.Where(dci => dci.DerivativeContract.IsActive && dci.DerivativeContract.StartDate <= now && (dci.DerivativeContract.EndDate == null || dci.DerivativeContract.EndDate >= now) && dci.RemainingQuantity > 0);
                }

                var derivativeContractItems = query.ToArray();
                var derivativeContractInfos = derivativeContractItems.GroupBy(dci => new { dci.ProductId, dci.DerivativeContract.Type }).Select(dcig =>
                {
                    var derivativeContractInfo = AbstractTypeFactory<DerivativeContractInfo>.TryCreateInstance();
                    derivativeContractInfo.ProductId = dcig.Key.ProductId;
                    derivativeContractInfo.Type = EnumUtility.SafeParse(dcig.Key.Type, DerivativeContractType.Forward);
                    derivativeContractInfo.ContractSize = dcig.Sum(dci => dci.ContractSize);
                    derivativeContractInfo.PurchasedQuantity = dcig.Sum(dci => dci.PurchasedQuantity);
                    derivativeContractInfo.RemainingQuantity = dcig.Sum(dci => dci.RemainingQuantity);
                    return derivativeContractInfo;
                });

                return derivativeContractInfos.ToArray();
            }
        }

        #endregion
    }
}