using Microsoft.EntityFrameworkCore;
using Wallet.Service.Models.Entities;

namespace Wallet.Service.Data;

public class WalletDbContext : Campus.Infrastructure.BaseDbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options) { }

    public DbSet<UserWallet> UserWallets => Set<UserWallet>();
    public DbSet<WalletLog> WalletLogs => Set<WalletLog>();
    public DbSet<Withdraw> Withdraws => Set<Withdraw>();
}