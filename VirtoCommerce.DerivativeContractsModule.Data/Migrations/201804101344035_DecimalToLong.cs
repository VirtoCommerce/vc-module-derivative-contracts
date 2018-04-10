namespace VirtoCommerce.DerivativeContractsModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DecimalToLong : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DerivativeContractItem", "RemainingQuantity");
            AlterColumn("dbo.DerivativeContractItem", "ContractSize", c => c.Long(nullable: false));
            AlterColumn("dbo.DerivativeContractItem", "PurchasedQuantity", c => c.Long(nullable: false));
            Sql("ALTER TABLE dbo.DerivativeContractItem ADD RemainingQuantity AS ContractSize - PurchasedQuantity");
        }
        
        public override void Down()
        {
            DropColumn("dbo.DerivativeContractItem", "RemainingQuantity");
            AlterColumn("dbo.DerivativeContractItem", "PurchasedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.DerivativeContractItem", "ContractSize", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            Sql("ALTER TABLE dbo.DerivativeContractItem ADD RemainingQuantity AS ContractSize - PurchasedQuantity");
        }
    }
}
