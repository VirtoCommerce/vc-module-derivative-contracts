using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using VirtoCommerce.DerivativeContractsModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.DerivativeContractsModule.Data.Repositories
{
    public class DerivativeContractRepository : EFRepositoryBase, IDerivativeContractRepository
    {
        public DerivativeContractRepository()
        {
        }

        public DerivativeContractRepository(string nameOrConnectionString, params IInterceptor[] interceptors)
            : base(nameOrConnectionString, null, interceptors)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DerivativeContractEntity>().ToTable("DerivativeContract").HasKey(x => x.Id).Property(x => x.Id);

            modelBuilder.Entity<DerivativeContractItemEntity>().ToTable("DerivativeContractItem").HasKey(x => x.Id).Property(x => x.Id);
            modelBuilder.Entity<DerivativeContractItemEntity>().HasRequired(dc => dc.DerivativeContract).WithMany(dci => dci.Items).HasForeignKey(dc => dc.DerivativeContractId).WillCascadeOnDelete(true);
            modelBuilder.Entity<DerivativeContractItemEntity>().Property(t => t.DerivativeContractId)
                   .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_DerivativeContractItem_DerivativeContractId_FulfillmentCenterId_ProductId", 1) { IsUnique = true }));
            modelBuilder.Entity<DerivativeContractItemEntity>().Property(t => t.FulfillmentCenterId)
                   .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_DerivativeContractItem_DerivativeContractId_FulfillmentCenterId_ProductId", 2) { IsUnique = true }));
            modelBuilder.Entity<DerivativeContractItemEntity>().Property(t => t.ProductId)
                   .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_DerivativeContractItem_DerivativeContractId_FulfillmentCenterId_ProductId", 3) { IsUnique = true }));

            base.OnModelCreating(modelBuilder);
        }

        public IQueryable<DerivativeContractEntity> DerivativeContracts => GetAsQueryable<DerivativeContractEntity>();

        public DerivativeContractEntity[] GetDerivativeContractsByIds(string[] ids)
        {
            return DerivativeContracts.Where(dc => ids.Contains(dc.Id)).Include(dc => dc.Items).ToArray();
        }

        public void RemoveDerivativeContracts(string[] ids)
        {
            var items = GetDerivativeContractsByIds(ids);
            foreach (var item in items)
            {
                Remove(item);
            }
        }
    }
}
