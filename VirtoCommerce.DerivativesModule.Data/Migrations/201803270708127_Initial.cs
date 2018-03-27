namespace VirtoCommerce.DerivativesModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Derivative",
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
                "dbo.DerivativeItem",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DerivativeId = c.String(nullable: false, maxLength: 128),
                        FulfillmentCenterId = c.String(nullable: false, maxLength: 128),
                        ProductId = c.String(nullable: false, maxLength: 128),
                        ContractSize = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PurchasedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 64),
                        ModifiedBy = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Derivative", t => t.DerivativeId, cascadeDelete: true)
                .ForeignKey("dbo.FulfillmentCenter", t => t.FulfillmentCenterId, cascadeDelete: true)
                .Index(t => new { t.DerivativeId, t.FulfillmentCenterId, t.ProductId }, unique: true, name: "IX_DerivativeItem_DerivativeId_FulfillmentCenterId_ProductId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DerivativeItem", "FulfillmentCenterId", "dbo.FulfillmentCenter");
            DropForeignKey("dbo.DerivativeItem", "DerivativeId", "dbo.Derivative");
            DropIndex("dbo.DerivativeItem", "IX_DerivativeItem_DerivativeId_FulfillmentCenterId_ProductId");
            DropTable("dbo.DerivativeItem");
            DropTable("dbo.Derivative");
        }
    }
}
