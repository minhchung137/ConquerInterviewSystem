using ConquerInterviewBO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConquerInterviewDAO
{
    public class SubscriptionPlanDAO
    {
        private readonly ConquerInterviewDbContext _context;
        private static SubscriptionPlanDAO instance;

        public static SubscriptionPlanDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SubscriptionPlanDAO();
                }
                return instance;
            }
        }

        public SubscriptionPlanDAO()
        {
            _context = new ConquerInterviewDbContext();
        }

        // Create
        public async Task<SubscriptionPlan> CreateAsync(SubscriptionPlan plan)
        {
            await _context.SubscriptionPlans.AddAsync(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        // Read (All)
        public async Task<List<SubscriptionPlan>> GetAllAsync()
        {
            return await _context.SubscriptionPlans.ToListAsync();
        }

        // Read (By ID)
        public async Task<SubscriptionPlan?> GetByIdAsync(int planId)
        {
            return await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.PlanId == planId);
        }

        // Update
        public async Task UpdateAsync(SubscriptionPlan plan)
        {
            _context.Entry(plan).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Delete
        public async Task DeleteAsync(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Remove(plan);
            await _context.SaveChangesAsync();
        }
    }
}