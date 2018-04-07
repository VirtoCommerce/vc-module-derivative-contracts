namespace VirtoCommerce.DerivativeContractsModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DerivativeContract",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        MemberId = c.String(nullable: false, maxLength: 128),
                        Type = c.String(nullable: false, maxLength: 32),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 64),
                        ModifiedBy = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DerivativeContractItem",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DerivativeContractId = c.String(nullable: false, maxLength: 128),
                        FulfillmentCenterId = c.String(maxLength: 128),
                        ProductId = c.String(nullable: false, maxLength: 128),
                        ContractSize = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PurchasedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 64),
                        ModifiedBy = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DerivativeContract", t => t.DerivativeContractId, cascadeDelete: true)
                .Index(t => new { t.DerivativeContractId, t.FulfillmentCenterId, t.ProductId }, unique: true, name: "IX_DerivativeContractItem_DerivativeContractId_FulfillmentCenterId_ProductId");

            Sql("ALTER TABLE dbo.DerivativeContractItem ADD RemainingQuantity AS ContractSize - PurchasedQuantity");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DerivativeContractItem", "DerivativeContractId", "dbo.DerivativeContract");
            DropIndex("dbo.DerivativeContractItem", "IX_DerivativeContractItem_DerivativeContractId_FulfillmentCenterId_ProductId");
            DropTable("dbo.DerivativeContractItem");
            DropTable("dbo.DerivativeContract");
        }
    }
}
