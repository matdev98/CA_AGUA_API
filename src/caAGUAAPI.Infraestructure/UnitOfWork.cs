using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using caAGUAAPI.Application.Interfaces.Repositories;
using caAGUAAPI.Domain;
using caAGUAAPI.Domain.Entities;
using caAGUAAPI.Infraestructure.Persistence;
using caAGUAAPI.Infraestructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace caAGUAAPI.Infraestructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }


        // Iniciar una transacción
        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        // Confirmar la transacción
        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        // Revertir los cambios si hay un error
        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            _transaction?.Dispose();
        }
    }
}
