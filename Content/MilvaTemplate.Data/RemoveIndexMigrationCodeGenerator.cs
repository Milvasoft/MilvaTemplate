using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Milvasoft.Helpers.DataAccess.EfCore.Concrete.Entity;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;

namespace MilvaTemplate.Data;

/// <summary>
/// For remove <see cref="EntityPropertyNames.CreatorUserId"/>, <see cref="EntityPropertyNames.LastModifierUserId"/> and <see cref="EntityPropertyNames.DeleterUserId"/> FK Indexes.
/// </summary>
public class RemoveIndexMigrationCodeGenerator : NpgsqlMigrationsSqlGenerator
{
    /// <summary>
    /// For remove <see cref="EntityPropertyNames.CreatorUserId"/>, <see cref="EntityPropertyNames.LastModifierUserId"/> and <see cref="EntityPropertyNames.DeleterUserId"/> FK Indexes.
    /// </summary>
    /// <param name="dependencies"></param>
    /// <param name="npgsqlOptions"></param>
    public RemoveIndexMigrationCodeGenerator(MigrationsSqlGeneratorDependencies dependencies, INpgsqlOptions npgsqlOptions) : base(dependencies, npgsqlOptions)
    {
    }

    /// <summary>
    /// If index name equals to <see cref="EntityPropertyNames.CreatorUserId"/>, <see cref="EntityPropertyNames.LastModifierUserId"/> and <see cref="EntityPropertyNames.DeleterUserId"/> this 3 name then don't create index.
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="model"></param>
    /// <param name="builder"></param>
    /// <param name="terminate"></param>
    protected override void Generate(CreateIndexOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate = true)
    {
        if (operation.Name.Contains(EntityPropertyNames.CreatorUserId)
            || operation.Name.Contains(EntityPropertyNames.LastModifierUserId)
            || operation.Name.Contains(EntityPropertyNames.DeleterUserId))
            return;

        base.Generate(operation, model, builder, terminate);
    }
}
