using ChatApi.Data;
using ChatApi.Data.Entities;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace ChatApi.Data.Migrations;

[DbContext(typeof(ChatDbContext))]
public sealed class ChatDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder) => ConfigureModel(modelBuilder);

    internal static void ConfigureModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "11.0.0")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        modelBuilder.Entity<ConversationEntity>(entity =>
        {
            entity.HasKey(conversation => conversation.Id);
            entity.Property(conversation => conversation.Id).HasColumnType("uniqueidentifier");
            entity.Property(conversation => conversation.ClosedAt).HasColumnType("datetimeoffset");
            entity.Property(conversation => conversation.CreatedAt).HasColumnType("datetimeoffset");
            entity.Property(conversation => conversation.CustomerExternalId)
                .HasMaxLength(128)
                .IsRequired();
            entity.Property(conversation => conversation.CustomerName)
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(conversation => conversation.OrderCode).HasMaxLength(64);
            entity.Property(conversation => conversation.Status)
                .HasConversion<string>()
                .HasMaxLength(16);
            entity.Property(conversation => conversation.Subject).HasMaxLength(200);
            entity.HasIndex(conversation => conversation.CreatedAt);
            entity.HasIndex(conversation => conversation.Status);
            entity.ToTable("Conversations");
        });

        modelBuilder.Entity<ParticipantEntity>(entity =>
        {
            entity.HasKey(participant => participant.Id);
            entity.Property(participant => participant.Id).HasColumnType("uniqueidentifier");
            entity.Property(participant => participant.ConversationId).HasColumnType("uniqueidentifier");
            entity.Property(participant => participant.ExternalId)
                .HasMaxLength(128)
                .IsRequired();
            entity.Property(participant => participant.JoinedAt).HasColumnType("datetimeoffset");
            entity.Property(participant => participant.Name)
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(participant => participant.Role)
                .HasConversion<string>()
                .HasMaxLength(16);
            entity.HasIndex(participant => new { participant.ConversationId, participant.ExternalId });
            entity.ToTable("Participants");
        });

        modelBuilder.Entity<MessageEntity>(entity =>
        {
            entity.HasKey(message => message.Id);
            entity.Property(message => message.Id).HasColumnType("uniqueidentifier");
            entity.Property(message => message.Content)
                .HasMaxLength(4000)
                .IsRequired();
            entity.Property(message => message.ConversationId).HasColumnType("uniqueidentifier");
            entity.Property(message => message.ParticipantId).HasColumnType("uniqueidentifier");
            entity.Property(message => message.SentAt).HasColumnType("datetimeoffset");
            entity.HasIndex(message => new { message.ConversationId, message.SentAt });
            entity.HasIndex(message => message.ParticipantId);
            entity.ToTable("Messages");
        });

        modelBuilder.Entity<ConversationEntity>()
            .HasMany(conversation => conversation.Participants)
            .WithOne(participant => participant.Conversation)
            .HasForeignKey(participant => participant.ConversationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<ConversationEntity>()
            .HasMany(conversation => conversation.Messages)
            .WithOne(message => message.Conversation)
            .HasForeignKey(message => message.ConversationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<ParticipantEntity>()
            .HasMany(participant => participant.Messages)
            .WithOne(message => message.Participant)
            .HasForeignKey(message => message.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
