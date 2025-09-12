using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            // Set CreatedDate if property exists
            var createdDateProperty = typeof(T).GetProperty("CreatedDate");
            if (createdDateProperty != null)
            {
                createdDateProperty.SetValue(entity, DateTime.Now);
            }
            
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            // Set ModifiedDate if property exists
            var modifiedDateProperty = typeof(T).GetProperty("ModifiedDate");
            if (modifiedDateProperty != null)
            {
                modifiedDateProperty.SetValue(entity, DateTime.Now);
            }
            
            // Get the entity's primary key value
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null)
            {
                var idValue = idProperty.GetValue(entity);
                
                // Check if entity is already being tracked
                var trackedEntity = _context.ChangeTracker.Entries<T>()
                    .FirstOrDefault(e => idProperty.GetValue(e.Entity).Equals(idValue));
                
                if (trackedEntity != null)
                {
                    // Entity is already being tracked, update its properties
                    foreach (var property in typeof(T).GetProperties())
                    {
                        if (property.Name != "Id" && property.CanWrite)
                        {
                            var newValue = property.GetValue(entity);
                            property.SetValue(trackedEntity.Entity, newValue);
                        }
                    }
                    trackedEntity.State = EntityState.Modified;
                }
                else
                {
                    // Entity is not being tracked, attach it
                    _dbSet.Attach(entity);
                    _context.Entry(entity).State = EntityState.Modified;
                }
            }
            else
            {
                // Fallback to original Update method
                _dbSet.Update(entity);
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                // Check if entity has IsDeleted property
                var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
                var modifiedDateProperty = typeof(T).GetProperty("ModifiedDate");
                
                if (isDeletedProperty != null)
                {
                    isDeletedProperty.SetValue(entity, true);
                }
                
                if (modifiedDateProperty != null)
                {
                    modifiedDateProperty.SetValue(entity, DateTime.Now);
                }
                
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            // Use reflection to check if Id property exists
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null) return false;
            
            return await _dbSet.AnyAsync(e => EF.Property<int>(e, "Id") == id);
        }
    }
}
