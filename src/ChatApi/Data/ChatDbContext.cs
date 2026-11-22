using ChatApi.Data.Entities;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.Data;

public sealed class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
{
    internal DbSet<ConversationEntity> Conversations => Set<ConversationEntity>();
    internal DbSet<ParticipantEntity> Participants => Set<ParticipantEntity>();
    internal DbSet<MessageEntity> Messages => Set<MessageEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConversationEntity>(entity =>
        {
            entity.HasKey(conversation => conversation.Id);
            entity.Property(conversation => conversation.CustomerExternalId).HasMaxLength(128).IsRequired();
            entity.Property(conversation => conversation.CustomerName).HasMaxLength(200).IsRequired();
            entity.Property(conversation => conversation.OrderCode).HasMaxLength(64);
            entity.Property(conversation => conversation.Subject).HasMaxLength(200);
            entity.Property(conversation => conversation.Status).HasConversion<string>().HasMaxLength(16);
            entity.HasIndex(conversation => conversation.Status);
            entity.HasIndex(conversation => conversation.CreatedAt);
            entity.HasMany(conversation => conversation.Participants)
                .WithOne(participant => participant.Conversation)
                .HasForeignKey(participant => participant.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(conversation => conversation.Messages)
                .WithOne(message => message.Conversation)
                .HasForeignKey(message => message.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ParticipantEntity>(entity =>
        {
            entity.HasKey(participant => participant.Id);
            entity.Property(participant => participant.ExternalId).HasMaxLength(128).IsRequired();
            entity.Property(participant => participant.Name).HasMaxLength(200).IsRequired();
            entity.Property(participant => participant.Role).HasConversion<string>().HasMaxLength(16);
            entity.HasIndex(participant => new { participant.ConversationId, participant.ExternalId });
            entity.HasMany(participant => participant.Messages)
                .WithOne(message => message.Participant)
                .HasForeignKey(message => message.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MessageEntity>(entity =>
        {
            entity.HasKey(message => message.Id);
            entity.Property(message => message.Content).HasMaxLength(4000).IsRequired();
            entity.HasIndex(message => new { message.ConversationId, message.SentAt });
        });

        base.OnModelCreating(modelBuilder);
    }
}
