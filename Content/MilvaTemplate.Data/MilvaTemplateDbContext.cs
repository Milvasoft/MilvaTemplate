using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Milvasoft.Helpers.DataAccess.MilvaContext;
using Milvasoft.Helpers.DependencyInjection;
using Milvasoft.Helpers.Encryption.Abstract;
using Milvasoft.Helpers.Encryption.Concrete;
using MilvaTemplate.Entity;
using MilvaTemplate.Entity.Identity;
using System;

namespace MilvaTemplate.Data
{
    /// <summary>
    /// DbContext class of MilvaTemplate project. This class handles all database operations.
    /// </summary>
    public class MilvaTemplateDbContext : MilvaDbContextBase<MilvaTemplateUser, MilvaTemplateRole, Guid>
    {

        private const string _key = "5u8x/A?D(G+KbPeS";
        private readonly IMilvaEncryptionProvider _provider;

        /// <summary>
        /// Cunstructor of <see cref="MilvaTemplateDbContext"/>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="auditConfiguration"></param>
        public MilvaTemplateDbContext(DbContextOptions<MilvaTemplateDbContext> options,
                                      IHttpContextAccessor httpContextAccessor,
                                      IAuditConfiguration auditConfiguration) : base(options, httpContextAccessor, auditConfiguration)
                                => _provider = new MilvaEncryptionProvider(_key);

        #region DbSets

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public DbSet<SystemLanguage> SystemLanguages { get; set; }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        #endregion

        /// <summary>
        /// Overrided the OnModelCreating for custom configurations to database.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region PostgreSql Configuration

            modelBuilder.UseDatabaseTemplate("template0");
            modelBuilder.UseCollation("C");
            modelBuilder.ConfigureStringProperties();
            modelBuilder.UseTablespace("pg_default");

            #endregion

            modelBuilder.Entity<MilvaTemplateUser>()
               .HasOne(p => p.DeleterUser)
               .WithMany()
               .HasForeignKey(p => p.DeleterUserId);

            modelBuilder.Entity<MilvaTemplateUser>()
                .HasOne(p => p.CreatorUser)
                .WithMany()
                .HasForeignKey(p => p.CreatorUserId);

            modelBuilder.Entity<MilvaTemplateUser>()
                .HasOne(p => p.LastModifierUser)
                .WithMany()
                .HasForeignKey(p => p.LastModifierUserId);

            //modelBuilder.UseEncryption(_provider);
            modelBuilder.ConfigureDecimalProperties();
            //modelBuilder.IgnoreDefaultRecords();
            modelBuilder.AddIndexToIndelibleEntities();
            modelBuilder.ConfigureDefaultValue();
            base.OnModelCreating(modelBuilder);
        }
    }
}
