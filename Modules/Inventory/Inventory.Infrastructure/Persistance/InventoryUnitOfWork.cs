using Framework.Persistance;
using Framework.Persistance.Interfaces;
using Inventory.Domain;
using Inventory.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Persistance
{
    public class InventoryUnitOfWork : UnitOfWork, ISecureUnitOfWork
    {
        public InventoryUnitOfWork(InventoryDbContext dbContext, Guid userId, List<string> roles) : base(dbContext, userId, roles)
        {

        }

        public async Task<bool> CanCreateAsync<T>(CancellationToken cancellationToken = default) where T : class, ISecureEntity
        {
            return await CheckPermissionAsync<T>(p => p.CanCreate, cancellationToken);
        }

        public async Task<bool> CanReadAsync<T>(CancellationToken cancellationToken = default) where T : class, ISecureEntity
        {
            return await CheckPermissionAsync<T>(p => p.CanRead, cancellationToken);
        }

        public async Task<bool> CanUpdateAsync<T>(CancellationToken cancellationToken = default) where T : class, ISecureEntity
        {
            return await CheckPermissionAsync<T>(p => p.CanUpdate, cancellationToken);
        }

        public async Task<bool> CanDeleteAsync<T>(CancellationToken cancellationToken = default) where T : class, ISecureEntity
        {
            return await CheckPermissionAsync<T>(p => p.CanDelete, cancellationToken);
        }

        public override IGenericRepository<T> GetRepository<T>() where T : class
        {
            if (repositories.ContainsKey(typeof(T)))
            {
                return (IGenericRepository<T>)repositories[typeof(T)];
            }

            Func<CancellationToken, Task<Expression<Func<T, bool>>>> securityPredicateDelegate = async (cancellationToken) =>
            {
                if (typeof(ISecureEntity).IsAssignableFrom(typeof(T)))
                {
                    return await CreateSecurityPredicateAsync<T>(cancellationToken); 
                }
                return null;
            };

            var repository = new GenericRepository<T>(dbContext, securityPredicateDelegate);
            repositories.Add(typeof(T), repository);

            return repository;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries<ISecureEntity>())
            {
                var entity = entry.Entity;

                if (entry.State == EntityState.Added && !await CanCreateAsync<ISecureEntity>(cancellationToken))
                {
                    throw new UnauthorizedAccessException($"User does not have permission to create {entity.GetType().Name}");
                }

                if (entry.State == EntityState.Modified && !await CanUpdateAsync<ISecureEntity>(cancellationToken))
                {
                    throw new UnauthorizedAccessException($"User does not have permission to update {entity.GetType().Name}");
                }

                if (entry.State == EntityState.Deleted && !await CanDeleteAsync<ISecureEntity>(cancellationToken))
                {
                    throw new UnauthorizedAccessException($"User does not have permission to delete {entity.GetType().Name}");
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task<bool> CheckPermissionAsync<T>(
            Expression<Func<InventoryRoleEntityTypePermission, bool>> permissionPredicate,
            CancellationToken cancellationToken = default) where T : class, ISecureEntity
        {
            var entityTypeName = typeof(T).Name;

            var entityType = await dbContext.Set<EntityType>()
                .FirstOrDefaultAsync(x => x.Name == entityTypeName, cancellationToken);

            if (entityType == null)
                return false;

            var permissionsQuery = dbContext.Set<InventoryRoleEntityTypePermission>()
                .Where(p => roles.Contains(p.Role.Name) && p.EntityTypeId == entityType.Id);

            var hasPermission = await permissionsQuery.AnyAsync(permissionPredicate, cancellationToken);

            return hasPermission;
        }


        protected async Task<Expression<Func<T, bool>>> CreateSecurityPredicateAsync<T>(CancellationToken cancellationToken) where T : class
        {
            if (!typeof(ISecureEntity).IsAssignableFrom(typeof(T)))
            {
                return entity => true; // No filtering needed
            }

            var entityTypeName = typeof(T).Name;

            var entityType = await dbContext.Set<EntityType>()
                .FirstOrDefaultAsync(x => x.Name == entityTypeName, cancellationToken);

            if (entityType == null)
            {
                return entity => false; // EntityType not found, deny all
            }

            var permissions = await dbContext.Set<InventoryRoleEntityTypePermission>()
                .Where(p => roles.Contains(p.Role.Name) && p.EntityTypeId == entityType.Id && p.CanRead)
                .ToListAsync(cancellationToken);

            if (permissions == null || !permissions.Any())
            {
                return entity => false; // No permissions, deny all
            }

            var parameter = Expression.Parameter(typeof(T), "entity");
            Expression finalExpression = null;

            foreach (var permission in permissions)
            {
                var permissionCondition = BuildPermissionCondition<T>(parameter, permission);
                finalExpression = finalExpression == null
                    ? permissionCondition
                    : Expression.OrElse(finalExpression, permissionCondition);
            }

            var lambda = Expression.Lambda<Func<T, bool>>(finalExpression, parameter);
            return await Task.FromResult(lambda);
        }

        //protected async Task<Expression<Func<T, bool>>> CreateSecurityPredicateAsync<T>(
        //        Guid userId,
        //        List<InventoryRoleEntityTypePermission> entityPermissions,
        //        CancellationToken cancellationToken = default) where T : class
        //{
        //    if (!typeof(ISecureEntity).IsAssignableFrom(typeof(T)))
        //    {
        //        return x => true;
        //    }

        //    var readableEntityTypeIds = entityPermissions
        //        .Where(p => p.CanRead)
        //        .Select(p => p.EntityTypeId)
        //        .Distinct()
        //        .ToList();

        //    var mustCheckOwnOnlyEntityTypeIds = entityPermissions
        //        .Where(p => p.CanRead && p.CanReadOwnOnly)
        //        .Select(p => p.EntityTypeId)
        //        .Distinct()
        //        .ToList();

        //    var parameter = Expression.Parameter(typeof(T), "e");

        //    var entityTypeIdProp = Expression.Property(Expression.Convert(parameter, typeof(ISecureEntity)), nameof(ISecureEntity.EntityTypeId));
        //    var createdByProp = Expression.Property(Expression.Convert(parameter, typeof(ISecureEntity)), nameof(ISecureEntity.CreatedBy));

        //    var entityTypeIdContains = Expression.Call(
        //        Expression.Constant(readableEntityTypeIds),
        //        typeof(List<Guid>).GetMethod("Contains", new[] { typeof(Guid) })!,
        //        entityTypeIdProp
        //    );

        //    var ownOnlyEntityTypeContains = Expression.Call(
        //        Expression.Constant(mustCheckOwnOnlyEntityTypeIds),
        //        typeof(List<Guid>).GetMethod("Contains", new[] { typeof(Guid) })!,
        //        entityTypeIdProp
        //    );

        //    var createdByEquals = Expression.Equal(
        //        createdByProp,
        //        Expression.Constant(userId)
        //    );

        //    var ownOnlyCondition = Expression.AndAlso(
        //        ownOnlyEntityTypeContains,
        //        createdByEquals
        //    );

        //    var finalPredicate = Expression.OrElse(entityTypeIdContains, ownOnlyCondition);

        //    var lambda = Expression.Lambda<Func<T, bool>>(finalPredicate, parameter);

        //    return await Task.FromResult(lambda);
        //}

        private Expression BuildPermissionCondition<T>(ParameterExpression parameter, InventoryRoleEntityTypePermission permission) where T : class
        {
            // Build EntityTypeId condition
            var entityTypeIdProperty = Expression.Property(parameter, nameof(ISecureEntity.EntityTypeId));
            var entityTypeIdConstant = Expression.Constant(permission.EntityTypeId);
            var entityTypeIdCondition = Expression.Equal(entityTypeIdProperty, entityTypeIdConstant);

            if (permission.CanReadOwnOnly)
            {
                // Build CreatedBy == UserId condition
                var createdByProperty = Expression.Property(parameter, nameof(ISecureEntity.CreatedBy));
                var userIdConstant = Expression.Constant(userId);
                var createdByCondition = Expression.Equal(createdByProperty, userIdConstant);

                // Combine bothreturn Expression.AndAlso(entityTypeIdCondition, createdByCondition);
            }
            else
            {
                // Only EntityTypeId check
                return entityTypeIdCondition;
            }


            // Must remove
            return null;
        }
    }
}

