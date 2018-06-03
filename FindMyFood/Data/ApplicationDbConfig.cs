using FindMyFood.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FindMyFood.Data
{
    internal class AppUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder) {
            builder.ToTable("AppUsers");
            builder.Ignore(user => user.PhoneNumber);
            builder.Ignore(user => user.PhoneNumberConfirmed);
            builder.Property(user => user.UserName).IsRequired();
            builder.Property(user => user.NormalizedUserName).IsRequired();
            builder.Property(user => user.Email).IsRequired();
            builder.Property(user => user.NormalizedUserName).IsRequired();
            builder.HasIndex(e => e.ClientId);
            builder.HasIndex(e => e.RestaurantId);
            builder.HasOne(d => d.Restaurant)
                .WithOne(user => user.ApplicationUser)
                .OnDelete(DeleteBehavior.Cascade).IsRequired(false);
            builder.HasOne(d => d.Client)
                .WithOne(user => user.ApplicationUser)
                .OnDelete(DeleteBehavior.Cascade).IsRequired(false);
            builder.HasIndex(e => new {e.Email})
                .HasName("EmailUnique")
                .IsUnique();
            builder.HasIndex(e => new {e.NormalizedEmail})
                .HasName("NormEmailUnique")
                .IsUnique();
        }
    }

    internal class RestaurantConfig : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder) {
            builder.ToTable("Restaurants");
            builder.Property(user => user.Name).IsRequired();
            builder.Property(user => user.Address).IsRequired();
            builder.Property(user => user.Latitude).IsRequired();
            builder.Property(user => user.Longitude).IsRequired();
            //builder.Property(user => user.AppUser).IsRequired();
        }
    }

    internal class ClientConfig : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder) {
            builder.ToTable("Clients");
            builder.Property(user => user.Name).IsRequired();
            //builder.Property(user => user.AppUser).IsRequired();
        }
    }

    internal class AppRoleConfig : IEntityTypeConfiguration<IdentityRole<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<int>> builder) {
            builder.ToTable("AppRoles");
        }
    }

    internal class AppRoleClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<int>> builder) {
            builder.ToTable("AppRoleClaims");
        }
    }

    internal class AppUserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<int>> builder) {
            builder.ToTable("AppUserLogins");
        }
    }

    internal class AppUserClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<int>> builder) {
            builder.ToTable("AppUserClaims");
        }
    }

    internal class AppUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<int>> builder) {
            builder.ToTable("AppUserRoles");
        }
    }

    internal class AppUserTokenConfig : IEntityTypeConfiguration<IdentityUserToken<int>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<int>> builder) {
            builder.ToTable("AppUserTokens");
        }
    }

    internal class PromotionConfig : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder) {
            builder.ToTable("Promotions");
            builder.Property(promotion => promotion.Description).IsRequired();
            builder.Property(promotion => promotion.Tags).IsRequired();
            builder.Property(promotion => promotion.RepetitionMode).IsRequired();
            builder.Property(promotion => promotion.Monday).IsRequired();
            builder.Property(promotion => promotion.Tuesday).IsRequired();
            builder.Property(promotion => promotion.Wednesday).IsRequired();
            builder.Property(promotion => promotion.Thursday).IsRequired();
            builder.Property(promotion => promotion.Friday).IsRequired();
            builder.Property(promotion => promotion.Saturday).IsRequired();
            builder.Property(promotion => promotion.Sunday).IsRequired();
            builder.HasIndex(e => e.RestaurantId);
            builder.HasOne(d => d.Restaurant)
                .WithMany(p => p.Promotions).IsRequired()
                .HasForeignKey(d => d.RestaurantId);
        }
    }

    internal class RatingConfig : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder) {
            builder.ToTable("Ratings");
            builder.Property(rating => rating.Rate).IsRequired();

            builder.HasIndex(e => e.ClientId);

            builder.HasIndex(e => e.RestaurantId);

            builder.HasIndex(e => new {e.ClientId, e.RestaurantId})
                .HasName("RatUnique")
                .IsUnique();

            builder.HasOne(d => d.Client)
                .WithMany(p => p.Ratings).IsRequired()
                .HasForeignKey(d => d.ClientId);

            builder.HasOne(d => d.Restaurant)
                .WithMany(p => p.Ratings).IsRequired()
                .HasForeignKey(d => d.RestaurantId);
        }
    }
}