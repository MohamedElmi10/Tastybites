using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using TastyBites.Recipes;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace TastyBites.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class TastyBitesDbContext :
    AbpDbContext<TastyBitesDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    // TastyBites
    public DbSet<Recipe> Recipes { get; set; }

    #endregion

    public TastyBitesDbContext(DbContextOptions<TastyBitesDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(TastyBitesConsts.DbTablePrefix + "YourEntities", TastyBitesConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
         builder.Entity<Recipe>(b =>
        {
           b.ToTable(TastyBitesConsts.DbTablePrefix + "Recipes", TastyBitesConsts.DbSchema);
           b.ConfigureByConvention(); //auto configure for the base class props
           b.Property(r => r.ImageUrl).HasMaxLength(RecipeConsts.ImageUrlMaxLength); // nullable varchar(512)

           // One-to-many. Cascade: deleting a recipe deletes its ingredients and steps too.
           b.HasMany(r => r.Ingredients).WithOne().HasForeignKey(i => i.RecipeId).OnDelete(DeleteBehavior.Cascade);
           b.HasMany(r => r.Steps).WithOne().HasForeignKey(st => st.RecipeId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<RecipeIngredient>(b =>
        {
           b.ToTable(TastyBitesConsts.DbTablePrefix + "RecipeIngredients", TastyBitesConsts.DbSchema);
           b.ConfigureByConvention();
           b.Property(i => i.Name).IsRequired().HasMaxLength(RecipeConsts.IngredientNameMaxLength);
           b.Property(i => i.Unit).HasMaxLength(RecipeConsts.IngredientUnitMaxLength);
           b.Property(i => i.Amount).HasPrecision(10, 2); // e.g. 1234.50
        });

        builder.Entity<RecipeStep>(b =>
        {
           b.ToTable(TastyBitesConsts.DbTablePrefix + "RecipeSteps", TastyBitesConsts.DbSchema);
           b.ConfigureByConvention();
           b.Property(st => st.Text).IsRequired().HasMaxLength(RecipeConsts.StepTextMaxLength);
        });
    }
}
