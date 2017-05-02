using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using BizMall.Data.DBContexts;

namespace BizMall.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170311132843_AddKws")]
    partial class AddKws
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ArticleList.Models.CompanyModels.ExternalLink", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Link");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("ExternalLink");
                });

            modelBuilder.Entity("BizMall.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<int?>("CompanyId");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("BizMall.Models.CommonModels.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<byte[]>("ImageContent");

                    b.Property<string>("ImageMimeType");

                    b.Property<bool>("IsMain");

                    b.Property<bool>("ToDelete");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CategoryId");

                    b.Property<int>("CategoryType");

                    b.Property<string>("Description");

                    b.Property<string>("EnTitle");

                    b.Property<string>("HashTags");

                    b.Property<string>("Link");

                    b.Property<string>("Title");

                    b.Property<DateTime>("UpdateTime");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CategoryId");

                    b.Property<int>("CategoryType");

                    b.Property<string>("EnTitle")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountType");

                    b.Property<string>("ApplicationUserId");

                    b.Property<string>("ContactEmail");

                    b.Property<string>("Description");

                    b.Property<string>("Telephone");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId")
                        .IsUnique();

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.KW", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CategoryId");

                    b.Property<int>("CategoryType");

                    b.Property<string>("kw")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.HasKey("Id");

                    b.ToTable("KWs");
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.RelCompanyGood", b =>
                {
                    b.Property<int>("CompanyId");

                    b.Property<int>("GoodId");

                    b.HasKey("CompanyId", "GoodId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("GoodId");

                    b.ToTable("RelCompanyGood");
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.RelCompanyImage", b =>
                {
                    b.Property<int>("CompanyId");

                    b.Property<int>("ImageId");

                    b.HasKey("CompanyId", "ImageId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("ImageId");

                    b.ToTable("RelCompanyImage");
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.RelGoodExternalLink", b =>
                {
                    b.Property<int>("ExternalLinkId");

                    b.Property<int>("GoodId");

                    b.HasKey("ExternalLinkId", "GoodId");

                    b.HasIndex("ExternalLinkId");

                    b.HasIndex("GoodId");

                    b.ToTable("RelGoodExternalLink");
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.RelGoodImage", b =>
                {
                    b.Property<int>("ImageId");

                    b.Property<int>("GoodId");

                    b.HasKey("ImageId", "GoodId");

                    b.HasIndex("GoodId");

                    b.HasIndex("ImageId");

                    b.ToTable("RelGoodImage");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.Article", b =>
                {
                    b.HasOne("BizMall.Models.CompanyModels.Category", "Category")
                        .WithMany("Goods")
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.Category", b =>
                {
                    b.HasOne("BizMall.Models.CompanyModels.Category", "ParentCategory")
                        .WithMany()
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.Company", b =>
                {
                    b.HasOne("BizMall.Models.ApplicationUser", "User")
                        .WithOne("Company")
                        .HasForeignKey("BizMall.Models.CompanyModels.Company", "ApplicationUserId");
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.RelCompanyGood", b =>
                {
                    b.HasOne("BizMall.Models.CompanyModels.Company", "Company")
                        .WithMany("Goods")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BizMall.Models.CompanyModels.Article", "Good")
                        .WithMany("Companies")
                        .HasForeignKey("GoodId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.RelCompanyImage", b =>
                {
                    b.HasOne("BizMall.Models.CompanyModels.Company", "Company")
                        .WithMany("Images")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BizMall.Models.CommonModels.Image", "Image")
                        .WithMany("Companies")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.RelGoodExternalLink", b =>
                {
                    b.HasOne("ArticleList.Models.CompanyModels.ExternalLink", "ExternalLink")
                        .WithMany("Articles")
                        .HasForeignKey("ExternalLinkId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BizMall.Models.CompanyModels.Article", "Good")
                        .WithMany("ExternalLinks")
                        .HasForeignKey("GoodId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BizMall.Models.CompanyModels.RelGoodImage", b =>
                {
                    b.HasOne("BizMall.Models.CompanyModels.Article", "Good")
                        .WithMany("Images")
                        .HasForeignKey("GoodId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BizMall.Models.CommonModels.Image", "Image")
                        .WithMany("Articles")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("BizMall.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("BizMall.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BizMall.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
