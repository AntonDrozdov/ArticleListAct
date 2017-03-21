using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using BizMall.Models;
using BizMall.Models.CommonModels;
using BizMall.Models.CompanyModels;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BizMall.Data.DBContexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<KW> KWs{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ////Remove cascading deletes
            //foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            //{
            //    relationship.DeleteBehavior = DeleteBehavior.Restrict;
            //}

            builder.Entity<ApplicationUser>()
                .HasOne(a => a.Company)
                .WithOne(c => c.User)
                .HasForeignKey<Company>(c => c.ApplicationUserId);

            //внешиние ссылки товара
            builder.Entity<RelGoodExternalLink>().HasKey(r => new { r.ExternalLinkId, r.GoodId});
            builder.Entity<RelGoodExternalLink>()
                .HasOne(r => r.ExternalLink)
                .WithMany(l => l.Articles)
                .HasForeignKey(r => r.ExternalLinkId);
            builder.Entity<RelGoodExternalLink>()
                .HasOne(r => r.Good)
                .WithMany(l => l.ExternalLinks)
                .HasForeignKey(r => r.GoodId);

            //изображения товара
            builder.Entity<RelGoodImage>().HasKey(r => new { r.ImageId, r.GoodId });
            builder.Entity<RelGoodImage>()
                .HasOne(r => r.Image)
                .WithMany(l => l.Articles)
                .HasForeignKey(r => r.ImageId);
            builder.Entity<RelGoodImage>()
                .HasOne(r => r.Good)
                .WithMany(l => l.Images)
                .HasForeignKey(r => r.GoodId);

            //товары компании
            builder.Entity<RelCompanyGood>().HasKey(r => new { r.CompanyId, r.GoodId });
            builder.Entity<RelCompanyGood>()
                .HasOne(r => r.Company)
                .WithMany(l => l.Goods)
                .HasForeignKey(r => r.CompanyId);
            builder.Entity<RelCompanyGood>()
                .HasOne(r => r.Good)
                .WithMany(l => l.Companies)
                .HasForeignKey(r => r.GoodId);

            //изображения компании
            builder.Entity<RelCompanyImage>().HasKey(r => new { r.CompanyId, r.ImageId});
            builder.Entity<RelCompanyImage>()
                .HasOne(r => r.Company)
                .WithMany(l => l.Images)
                .HasForeignKey(r => r.CompanyId);
            builder.Entity<RelCompanyImage>()
                .HasOne(r => r.Image)
                .WithMany(l => l.Companies)
                .HasForeignKey(r => r.ImageId);
        }
    }
}
