using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using VirtoCommerce.DerivativesModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;

namespace VirtoCommerce.DerivativesModule.Data.Repositories
{
    public class DerivativeRepository : EFRepositoryBase, IDerivativeRepository
    {
        public DerivativeRepository()
        {
        }

        public DerivativeRepository(string nameOrConnectionString, params IInterceptor[] interceptors)
            : base(nameOrConnectionString, null, interceptors)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DerivativeEntity>().ToTable("Derivative").HasKey(x => x.Id).Property(x => x.Id);
            modelBuilder.Entity<DerivativeItemEntity>().ToTable("DerivativeItem").HasKey(x => x.Id).Property(x => x.Id);

            modelBuilder.Entity<DerivativeItemEntity>()
              .HasRequired(x => x.Derivative).WithMany(x => x.Items)
              .HasForeignKey(x => x.DerivativeId);

            modelBuilder.Entity<DerivativeItemEntity>()
              .HasRequired(x => x.FulfillmentCenter).WithMany()
              .HasForeignKey(x => x.FulfillmentCenterId);

            modelBuilder.Entity<DerivativeItemEntity>().Property(t => t.DerivativeId)
                   .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_DerivativeItem_DerivativeId_FulfillmentCenterId_ProductId", 1) { IsUnique = true }));
            modelBuilder.Entity<DerivativeItemEntity>().Property(t => t.FulfillmentCenterId)
                   .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_DerivativeItem_DerivativeId_FulfillmentCenterId_ProductId", 2) { IsUnique = true }));
            modelBuilder.Entity<DerivativeItemEntity>().Property(t => t.ProductId)
                   .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_DerivativeItem_DerivativeId_FulfillmentCenterId_ProductId", 3) { IsUnique = true }));

            base.OnModelCreating(modelBuilder);
        }

        public IQueryable<DerivativeEntity> Derivatives => GetAsQueryable<DerivativeEntity>();

        public DerivativeEntity[] GetDerivativesByIds(IEnumerable<string> ids)
        {
            return Derivatives.Where(x => ids.Contains(x.Id)).Include(x => x.Items).ToArray();
        }

        public void RemoveDerivativesByIds(IEnumerable<string> ids)
        {
            var items = GetDerivativesByIds(ids);
            foreach (var item in items)
            {
                Remove(item);
            }
        }
    }
}
